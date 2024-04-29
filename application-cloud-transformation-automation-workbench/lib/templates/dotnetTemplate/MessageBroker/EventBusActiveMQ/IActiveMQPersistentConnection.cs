global using Microsoft.Extensions.Logging;
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

public interface IActiveMQPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    ISession CreateSession();
}
