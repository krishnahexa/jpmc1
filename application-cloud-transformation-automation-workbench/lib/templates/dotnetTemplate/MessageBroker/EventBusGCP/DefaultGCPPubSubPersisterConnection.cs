using Infrastructure.Common.EventBus;
using Infrastructure.Common.EventBus.Abstractions;
using Infrastructure.Common.EventBus.Events;
using Infrastructure.Common.EventBus.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using Autofac;
using System.Text;
using System.Text.Json;
using Google.Cloud.PubSub.V1;

namespace Infrastructure.Common.EventBusGCP;

public class DefaultGCPPubSubPersisterConnection : IGCPPubSubPersisterConnection
{
    private TopicName _topicName;
    private SubscriptionName _subscriptionName;
    private PublisherClient _publisherClient;
    private SubscriberClient _subscriberClient;

    bool _disposed;

    public DefaultGCPPubSubPersisterConnection(string projectId, string topicId, string subscriptionId)
    {

        _topicName = TopicName.FromProjectTopic(projectId, topicId);
        _subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        _subscriberClient = SubscriberClient.Create(_subscriptionName);
         var publisherSetting = new PublisherClient.Settings();
         publisherSetting.EnableMessageOrdering = true;
        _publisherClient =  PublisherClient.Create(_topicName, null, publisherSetting);
       
    }

    public PublisherClient PublisherClient
    {
        get
        {
            if (_publisherClient == null)
            {
                _publisherClient = PublisherClient.Create(_topicName);
            }
            return _publisherClient;
        }
    }

    public SubscriberClient SubscriberClient
    {
        get
        {
            return SubscriberClient.Create(_subscriptionName);;
        }
    }

public TopicName TopicName
{
        get
        {
            return _topicName;
        }
    }
    public SubscriptionName SubscriptionName
    {
        get
        {
            return _subscriptionName;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _publisherClient = null;
    }
}
