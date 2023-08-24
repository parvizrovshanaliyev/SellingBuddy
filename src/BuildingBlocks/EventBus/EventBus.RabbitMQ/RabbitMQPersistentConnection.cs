using System;
using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly int _retryCount;
    private IConnection _connection;
    private bool _isDisposed;
    private readonly object lock_object = new();

    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        _retryCount = retryCount;
    }

    public bool IsConnected => _connection is { IsOpen: true };

    public void Dispose()
    {
        _isDisposed = true;
        _connection.Dispose();
    }

    public IModel CreateModel()
    {
        return _connection.CreateModel();
    }

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { });

            policy.Execute(() => { _connection = _connectionFactory.CreateConnection(); });

            if (IsConnected)
            {
                _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                _connection.CallbackException += Connection_CallbackException;
                _connection.ConnectionBlocked += Connection_ConnectionBlocked;
                // log
                return true;
            }

            // log
            return false;
        }
    }

    private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        // log Connection_ConnectionShutDown
        if (_isDisposed) return;

        TryConnect();
        throw new NotImplementedException();
    }

    private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        // log Connection_ConnectionShutDown
        if (_isDisposed) return;
        TryConnect();
        throw new NotImplementedException();
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // log Connection_ConnectionShutDown
        if (_isDisposed) return;
        TryConnect();

        throw new NotImplementedException();
    }
}