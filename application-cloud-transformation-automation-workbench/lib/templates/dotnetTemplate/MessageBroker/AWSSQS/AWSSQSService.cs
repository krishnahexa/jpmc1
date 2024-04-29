using System.Text.Json;
using System.Threading;
using Amazon.SQS;
using Amazon.SQS.Model;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Common.EventBus.AWSSQS;

public class AWSSQSService : IEventBus, IDisposable
{

    private readonly string _queueUrl;
    private readonly IAWSSQSPersisterConnection _serviceBusPersisterConnection;
    private readonly ILogger<AWSSQSService> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly string AUTOFAC_SCOPE_NAME = "event_bus";
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

    public AWSSQSService(IAWSSQSPersisterConnection serviceBusPersisterConnection,
        ILogger<AWSSQSService> logger, IEventBusSubscriptionsManager subsManager,
         ILifetimeScope autofac, string queueUrl)
    {
        _serviceBusPersisterConnection = serviceBusPersisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _autofac = autofac;
        _queueUrl = queueUrl;
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        // var body = System.Text.Encoding.UTF8.GetBytes(jsonMessage);


        var sendRequest = new SendMessageRequest(_queueUrl, jsonMessage);


        _serviceBusPersisterConnection.AWSSqs.SendMessageAsync(sendRequest)
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
                RegisterSubscriptionClientMessageHandlerAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
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
            _serviceBusPersisterConnection
                .AWSSqs
                .Dispose();

        }
        catch (Exception) 
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
        _serviceBusPersisterConnection.AWSSqs.Dispose();
    }

    private async Task<int> RegisterSubscriptionClientMessageHandlerAsync()
    {
        int messageCount = 0;
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
        };
        //CheckIs there any new message available to process
        var data = await _serviceBusPersisterConnection.AWSSqs.ReceiveMessageAsync(request);
        messageCount = data.Messages.Count();
        foreach (var message in data.Messages)
        {
            var eventName = $"{message.ReceiptHandle}{INTEGRATION_EVENT_SUFFIX}";
            string messageData = message.Body;

            // Complete the message so that it is not received again.
            var result = await ProcessEvent(eventName, messageData);


        }
        _serviceBusPersisterConnection.AWSSqs.Dispose();

        return messageCount;



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