using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Shared.Extensions;
using MediatR;

namespace OrderService.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContextDesignFactory()
    {
        
    }
    
    public OrderDbContext CreateDbContext(string[] args)
    {
        var connectionString = ConfigurationExtensions.GetDbConnectionStringFromConfiguration();
        
        Console.WriteLine($"DesignTime DbContext: {connectionString}");
        
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(connectionString);

        return new OrderDbContext(optionsBuilder.Options, new NoMediator());
    }

   
}

class NoMediator : IMediator
{
    public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken) where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        return Task.FromResult<TResponse>(default);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
    {
        return Task.CompletedTask;
    }

    public Task<object> Send(object request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(default);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return CreateStream(request, cancellationToken);
    }

    public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = new CancellationToken())
    {
        return CreateStream(request, cancellationToken);
    }
}