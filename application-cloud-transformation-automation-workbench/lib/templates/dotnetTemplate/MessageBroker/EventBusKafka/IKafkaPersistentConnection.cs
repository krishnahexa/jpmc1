global using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.Kafka.SyncOverAsync;
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

public interface IKafkaPersistentConnection
    : IDisposable
{
    public IProducer<string, byte[]> ProducerClient { get; }
    public IConsumer<string, byte[]> ConsumerClient { get; }

}
