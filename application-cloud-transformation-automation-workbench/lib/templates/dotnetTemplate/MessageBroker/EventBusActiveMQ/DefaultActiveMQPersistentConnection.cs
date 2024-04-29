using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
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

public class DefaultActiveMQPersistentConnection
    : IActiveMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultActiveMQPersistentConnection> _logger;
    private readonly int _retryCount;
    IConnection _connection;
    bool _disposed;

    object sync_root = new object();

    public DefaultActiveMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultActiveMQPersistentConnection> logger, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public bool IsConnected
    {
        get
        {
            return _connection != null && _connection.IsStarted && !_disposed;
        }
    }

    public ISession CreateSession()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No ActiveMQ connections are available to perform this action");
        }

        return _connection.CreateSession();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
           _connection.ExceptionListener += OnExceptionListener;
           _connection.ConnectionInterruptedListener += OnConnectionInterruptedListener;
           _connection.ConnectionResumedListener += OnConnectionResumedListener;
           _connection.Dispose();
        }
        catch (Apache.NMS.ActiveMQ.IOException ex)
        {
            _logger.LogCritical(ex.ToString());
        }
    }

    public bool TryConnect()
    {
        _logger.LogInformation("ActiveMQ Client is trying to connect");

        lock (sync_root)
        {
            var policy = RetryPolicy.Handle<SocketException>()
                .Or<Apache.NMS.ActiveMQ.ConnectionFailedException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "ActiveMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                }
            );

            policy.Execute(() =>
            {
                _connection = _connectionFactory
                        .CreateConnection();
            });

            if (IsConnected)
            {
                
                _connection.ExceptionListener += OnExceptionListener;
                _connection.ConnectionInterruptedListener += OnConnectionInterruptedListener;
                _connection.ConnectionResumedListener += OnConnectionResumedListener;

                _logger.LogInformation("ActiveMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.ClientId);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: ActiveMQ connections could not be created and opened");

                return false;
            }
        }
    }

    private void OnConnectionResumedListener()
    {
        if (_disposed) return;

        _logger.LogWarning("A ActiveMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }

    void OnExceptionListener(Exception  e)
    {
        if (_disposed) return;

        _logger.LogWarning("A ActiveMQ connection throw exception. Trying to re-connect...");

        TryConnect();
    }

    void OnConnectionInterruptedListener()
    {
        if (_disposed) return;

        _logger.LogWarning("A ActiveMQ connection is on shutdown. Trying to re-connect...");

        TryConnect();
    }
}
