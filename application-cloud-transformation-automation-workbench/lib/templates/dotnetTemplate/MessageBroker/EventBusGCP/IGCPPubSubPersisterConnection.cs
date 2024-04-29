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

public interface IGCPPubSubPersisterConnection : IDisposable
{
    TopicName TopicName { get; }
    SubscriptionName SubscriptionName { get; }
    PublisherClient PublisherClient { get; }
    SubscriberClient SubscriberClient { get; }
}