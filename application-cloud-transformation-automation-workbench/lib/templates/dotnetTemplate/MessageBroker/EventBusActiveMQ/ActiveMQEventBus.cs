using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using Apache.NMS.Policies;
using System;
using System.IO;
using System.Net.Sockets;
using Autofac;
using Infrastructure.Common.EventBus;
using Infrastructure.Common.EventBus.Abstractions;
using Infrastructure.Common.EventBus.Events;
using Infrastructure.Common.EventBus.Extensions;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Infrastructure.Common.EventBusActiveMQ;

public class ActiveMQEventBus : IEventBus, IDisposable
{
    const string BROKER_NAME = "";
    const string AUTOFAC_SCOPE_NAME = "Common";

    private readonly IActiveMQPersistentConnection _persistentConnection;
    private readonly ILogger<ActiveMQEventBus> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;

    private ISession _consumerChannel;
    private string _queueName;

    public ActiveMQEventBus(IActiveMQPersistentConnection persistentConnection, ILogger<ActiveMQEventBus> logger,
        ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _queueName = queueName;
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using var channel = _persistentConnection.CreateSession();
        channel.Unsubscribe(_queueName);

        if (_subsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = RetryPolicy.Handle<Apache.NMS.ActiveMQ.ConnectionFailedException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
            });

        var eventName = @event.GetType().Name;

        _logger.LogTrace("Creating ActiveMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        var channel = _persistentConnection.CreateSession();
        _logger.LogTrace("Declaring ActiveMQ exchange to publish event: {EventId}", @event.Id);

        var destination = channel.GetQueue(_queueName);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });

        policy.Execute(() =>
        {
            var producer = channel.CreateProducer(destination);
            producer.DeliveryMode = MsgDeliveryMode.Persistent; // persistent
           
            var message = producer.CreateBytesMessage(body);
           message.NMSCorrelationID = eventName;

            _logger.LogTrace("Publishing event to ActiveMQ: {EventId}", @event.Id);
            producer.Send(message);
        });
    }

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        DoInternalSubscription(eventName);
        _subsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsume();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        DoInternalSubscription(eventName);

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume();
    }

    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
 
            _consumerChannel.Recover();
        }
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }

        _subsManager.Clear();
    }

    private void StartBasicConsume()
    {
        _logger.LogTrace("Starting ActiveMQ basic consume");

        if (_consumerChannel != null)
        {
            IDestination destination =  _consumerChannel.GetQueue(_queueName);
            var consumer = _consumerChannel.CreateConsumer(destination);

            consumer.Listener  += new MessageListener(Consumer_Received);

            
        }
        else
        {
            _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    private void Consumer_Received(IMessage receivedMsg)
    {
        var eventName = receivedMsg.NMSCorrelationID;
        var message = Encoding.UTF8.GetString((receivedMsg as IBytesMessage).Content);

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.Activemq.com/dlx.html
        _consumerChannel.Acknowledge();
    }

    private ISession CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        _logger.LogTrace("Creating ActiveMQ consumer channel");

        var channel = _persistentConnection.CreateSession();

        IDestination destination = channel.GetQueue(_queueName);


        IMessageConsumer msgConsumer = channel.CreateConsumer(destination);

        return channel;
    }

    private void ProcessEvent(string eventName, string message)
    {
        _logger.LogTrace("Processing ActiveMQ event: {EventName}", eventName);

        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                if (subscription.IsDynamic)
                {
                    if (scope.ResolveOptional(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;
                    using dynamic eventData = JsonDocument.Parse(message);
                    //await Task.Yield();
                     handler.Handle(eventData);
                }
                else
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    ///await Task.Yield();
                    concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
        else
        {
            _logger.LogWarning("No subscription for ActiveMQ event: {EventName}", eventName);
        }
    }
}
