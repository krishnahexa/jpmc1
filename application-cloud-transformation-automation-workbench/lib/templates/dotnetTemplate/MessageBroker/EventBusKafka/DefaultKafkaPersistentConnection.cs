using Microsoft.Extensions.Logging;
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

public class DefaultKafkaPersistentConnection
    : IKafkaPersistentConnection
{
    private readonly ClientConfig _clientConfig;
    private IProducer<string, byte[]> _producerClient;
    private IConsumer<string, byte[]> _consumerClient;

    bool _disposed;

    public DefaultKafkaPersistentConnection(ClientConfig clientConfig)
    {
        _clientConfig = clientConfig;
        _producerClient = new ProducerBuilder<string, byte[]>(_clientConfig).SetValueSerializer(Serializers.ByteArray).Build();
        _consumerClient = new ConsumerBuilder<string, byte[]>(_clientConfig).SetValueDeserializer(Deserializers.ByteArray).Build();
    }

    public IProducer<string, byte[]> ProducerClient
    {
        get
        {
            if (_producerClient == null)
            {
                _producerClient = new ProducerBuilder<string, byte[]>(_clientConfig).Build();
            }
            return _producerClient;
        }
    }

   public IConsumer<string, byte[]> ConsumerClient
    {
        get
        {
            if (_consumerClient == null)
            {
                _consumerClient = new ConsumerBuilder<string, byte[]>(_clientConfig).Build();
            }
            return _consumerClient;
        }
    }
    
    

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _producerClient.Dispose();
        _consumerClient.Dispose();
    }
}
