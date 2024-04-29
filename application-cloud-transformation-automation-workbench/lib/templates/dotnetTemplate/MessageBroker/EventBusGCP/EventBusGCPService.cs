using System.Threading;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace Infrastructure.Common.EventBusGCP;

public class EventBusGCPService : IEventBus, IDisposable
{
    private readonly IGCPPubSubPersisterConnection _serviceBusPersisterConnection;
    private readonly ILogger<EventBusGCPService> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly string AUTOFAC_SCOPE_NAME = "event_bus";
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

    public EventBusGCPService(IGCPPubSubPersisterConnection serviceBusPersisterConnection,
        ILogger<EventBusGCPService> logger, IEventBusSubscriptionsManager subsManager,
         ILifetimeScope autofac)
    {
        _serviceBusPersisterConnection = serviceBusPersisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _autofac = autofac;
        
        
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
       // var body = System.Text.Encoding.UTF8.GetBytes(jsonMessage);

        var message = new PubsubMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            OrderingKey = eventName,
            Data =  ByteString.CopyFromUtf8(jsonMessage),
            
        };

        _serviceBusPersisterConnection.PublisherClient.PublishAsync(message)
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
            catch (Exception ex)
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
                .SubscriberClient
                .StopAsync(CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
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
        _serviceBusPersisterConnection.SubscriberClient.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    private async Task<int> RegisterSubscriptionClientMessageHandlerAsync()
    {
        int messageCount =0;
         Task startTask = _serviceBusPersisterConnection.SubscriberClient.StartAsync(async (
            PubsubMessage message, CancellationToken cancel) =>
            {
                var eventName = $"{message.OrderingKey}{INTEGRATION_EVENT_SUFFIX}";
                ByteString messageData = message.Data;

                // Complete the message so that it is not received again.
                var result = await ProcessEvent(eventName, messageData);
                
                return Task.FromResult(result ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack).Result;
            });
            await _serviceBusPersisterConnection.SubscriberClient.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return messageCount;


        
    }

    public void Dispose()
    {
        _subsManager.Clear();
        
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent(string eventName, ByteString message)
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

                    using dynamic eventData = JsonDocument.Parse(message.ToStringUtf8());
                    await handler.Handle(eventData);
                }
                else
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message.ToStringUtf8(), eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
        processed = true;
        return processed;
    }

    
}