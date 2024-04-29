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
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Infrastructure.Common.EventBus.AWSSQS;

public class DefaultAWSSQSPersisterConnection : IAWSSQSPersisterConnection
{

    private IAmazonSQS _awsSqs;


    bool _disposed;

    public DefaultAWSSQSPersisterConnection(IAmazonSQS sqs)
    {
        _awsSqs = sqs;

    }

    public IAmazonSQS AWSSqs
    {
        get
        {
            return _awsSqs;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _awsSqs = null;
    }
}
