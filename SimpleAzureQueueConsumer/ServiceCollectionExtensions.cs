using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleAzureQueueConsumer;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the base services required for the Simple Azure Queue Consumer (SAQC) to the service collection. <br />
    /// Saqc uses a background service to poll Azure Storage Queues for messages and dispatches them to registered handlers.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    public static void AddSaqc(this IServiceCollection services, string connectionString)
    {
        SaqcBase.SetConnectionString(connectionString);
        services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        services.AddHostedService<SaqcHostedService>();
    }
    
    /// <summary>
    /// Adds the base services required for the Simple Azure Queue Consumer (SAQC) to the service collection. <br />
    /// Saqc uses a background service to poll Azure Storage Queues for messages and dispatches them to registered handlers.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the services to.</param>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    public static void AddSaqc(this WebApplicationBuilder builder, string connectionString)
    {
        SaqcBase.SetConnectionString(connectionString);
        builder.Services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        builder.Services.AddHostedService<SaqcHostedService>();
    }
    
    /// <summary>
    /// Adds a scoped SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <param name="queueName">The name of the queue to associate with the handler.</param>
    /// <param name="pollingRateMs">The polling rate in milliseconds. Default is 5000 ms.</param>
    public static void AddSaqcHandler<THandler>(this IServiceCollection services, string queueName, int pollingRateMs = 5000)
        where THandler : class, IQueueMessageHandler
    {
        SaqcBase.AddQueueName(queueName, pollingRateMs);
        services.AddKeyedScoped<IQueueMessageHandler, THandler>(queueName);
    }
    
    /// <summary>
    /// Adds a scoped SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="builder">The WebApplicationBuilder to add the handler to.</param>
    /// <param name="queueName">The name of the queue to associate with the handler.</param>
    /// <param name="pollingRateMs">The polling rate in milliseconds. Default is 5000 ms.</param>
    public static void AddSaqcHandler<THandler>(this WebApplicationBuilder builder, string queueName, int pollingRateMs = 5000)
        where THandler : class, IQueueMessageHandler
    {
        SaqcBase.AddQueueName(queueName, pollingRateMs);
        builder.Services.AddKeyedScoped<IQueueMessageHandler, THandler>(queueName);
    }
}
