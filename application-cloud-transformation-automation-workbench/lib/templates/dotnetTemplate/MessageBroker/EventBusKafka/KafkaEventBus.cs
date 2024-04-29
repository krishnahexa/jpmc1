global using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Confluent.Kafka;
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

namespace Infrastructure.Common.EventBusKafka;

public class KafkaEventBus : IEventBus, IDisposable
{
     private readonly IKafkaPersistentConnection _kafkaPersisterConnection;
    private readonly ILogger<KafkaEventBus> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly string _topicName = "eshop_event_bus";
    private readonly string _subscriptionName;
    private IProducer<string, byte[]> _sender;
    private IConsumer<string, byte[]> _processor;
    private readonly string AUTOFAC_SCOPE_NAME = "eshop_event_bus";
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

    public KafkaEventBus(IKafkaPersistentConnection kafkaPersisterConnection,
        ILogger<KafkaEventBus> logger, IEventBusSubscriptionsManager subsManager, ILifetimeScope autofac, string subscriptionClientName)
    {
        _kafkaPersisterConnection = kafkaPersisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _autofac = autofac;
        _subscriptionName = subscriptionClientName;
        _sender = _kafkaPersisterConnection.ProducerClient;
        _processor = _kafkaPersisterConnection.ConsumerClient;

        RegisterSubscriptionClientMessageHandlerAsync().GetAwaiter().GetResult();
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var message = new Message<string, byte[]>();
        
        message.Key = eventName;
        message.Value = body;
        _sender.ProduceAsync(_topicName, message)
            .GetAwaiter()
            .GetResult();
    }

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        _subsManager.AddDynamicSubscription<TH>(eventName);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

        var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
        if (!containsKey)
        {
            try
            {
                _kafkaPersisterConnection.ConsumerClient.Subscribe(_topicName);
            }
            catch (Confluent.Kafka.KafkaException ex)
            {
                _logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
            }
        }

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        _subsManager.AddSubscription<T, TH>();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

        try
        {
            _kafkaPersisterConnection
                .ConsumerClient
                .Unsubscribe();
        }
        catch (Confluent.Kafka.KafkaException ex) when (ex.Error == ErrorCode.BrokerNotAvailable)
        {
            _logger.LogWarning("The messaging entity {eventName} Could not be found.", eventName);
        }

        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.LogInformation("Unsubscribing from dynamic event {EventName}", eventName);

        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    private async Task RegisterSubscriptionClientMessageHandlerAsync()
    {
        var args = _processor.Consume();

                var eventName = $"{args.Key}{INTEGRATION_EVENT_SUFFIX}";
                string messageData = args.Message.ToString();

                // Complete the message so that it is not received again.
                await ProcessEvent(eventName, messageData);
       
    }

    public void Dispose()
    {
        _subsManager.Clear();
    }

    private async Task<bool> ProcessEvent(string eventName, string message)
    {
        var processed = false;
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                if (subscription.IsDynamic)
                {
                    if (scope.ResolveOptional(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;

                    using dynamic eventData = JsonDocument.Parse(message);
                    await handler.Handle(eventData);
                }
                else
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
        processed = true;
        return processed;
    }
}