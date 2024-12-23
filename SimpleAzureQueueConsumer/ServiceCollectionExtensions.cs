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
    public static void AddSaqcBase(this IServiceCollection services, string connectionString)
    {
        SaqcBase.SetConnectionString(connectionString);
        services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        services.AddHostedService<SaqcHostedService>();
    }

    /// <summary>
    /// Adds a singleton SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <param name="queueName">The name of the queue to associate with the handler.</param>
    /// <param name="pollingRateMs">The polling rate in milliseconds. Default is 5000 ms.</param>
    public static void AddSaqcHandlerSingleton<THandler>(this IServiceCollection services, string queueName, int pollingRateMs = 5000)
        where THandler : class, IQueueMessageHandler
    {
        SaqcBase.AddQueueName(queueName, pollingRateMs);
        services.AddKeyedSingleton<IQueueMessageHandler, THandler>(queueName);
    }

    /// <summary>
    /// Adds a transient SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <param name="queueName">The name of the queue to associate with the handler.</param>
    /// <param name="pollingRateMs">The polling rate in milliseconds. Default is 5000 ms.</param>
    public static void AddSaqcHandlerTransient<THandler>(this IServiceCollection services, string queueName, int pollingRateMs = 5000)
        where THandler : class, IQueueMessageHandler
    {
        SaqcBase.AddQueueName(queueName, pollingRateMs);
        services.AddKeyedTransient<IQueueMessageHandler, THandler>(queueName);
    }

    /// <summary>
    /// Adds a scoped SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <param name="queueName">The name of the queue to associate with the handler.</param>
    /// <param name="pollingRateMs">The polling rate in milliseconds. Default is 5000 ms.</param>
    public static void AddSaqcHandlerScoped<THandler>(this IServiceCollection services, string queueName, int pollingRateMs = 5000)
        where THandler : class, IQueueMessageHandler
    {
        SaqcBase.AddQueueName(queueName, pollingRateMs);
        services.AddKeyedScoped<IQueueMessageHandler, THandler>(queueName);
    }
}
