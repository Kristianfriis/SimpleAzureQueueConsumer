using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleAzureQueueConsumer.Interfaces;
using SimpleAzureQueueConsumer.Models;

namespace SimpleAzureQueueConsumer.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the base services required for the Simple Azure Queue Consumer (SAQC) to the service collection. <br />
    /// Saqc uses a background service to poll Azure Storage Queues for messages and dispatches them to registered handlers.<br />
    ///
    /// This is the most basic way to configure the SAQC. You must provide the connection string for the Azure Storage account.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the services to.</param>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    public static void AddSaqc(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.Configure<SaqcOptions>(options =>
        {
            options.ConnectionString = connectionString;
        });
        
        builder.Services.AddSingleton<ISaqc, Saqc>();
        builder.Services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        builder.Services.AddHostedService<SaqcHostedService>();
    }
    
    /// <summary>
    /// Adds the base services required for the Simple Azure Queue Consumer (SAQC) to the service collection. <br />
    /// Saqc uses a background service to poll Azure Storage Queues for messages and dispatches them to registered handlers.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the services to.</param>
    /// <param name="configureOptions">The options to configure the general configurations</param>
    public static WebApplicationBuilder AddSaqc(this WebApplicationBuilder builder, Action<SaqcOptions> configureOptions)
    {
        // Use the configured options to register the Thing service
        builder.Services.Configure(configureOptions);
        
        builder.Services.AddSingleton<ISaqc, Saqc>();
        builder.Services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        builder.Services.AddHostedService<SaqcHostedService>();

        return builder;
    }
    
    /// <summary>
    /// Adds the base services required for the Simple Azure Queue Consumer (SAQC) to the service collection. <br />
    /// Saqc uses a background service to poll Azure Storage Queues for messages and dispatches them to registered handlers.<br />
    /// 
    ///This method will configure the SAQC options using the provided configuration section in the appsettings.json.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the services to.</param>
    public static WebApplicationBuilder AddSaqc(this WebApplicationBuilder builder)
    {
        // Use the configured options to register the Thing service
        builder.Services.Configure<SaqcOptions>(builder.Configuration.GetSection(SaqcOptions.Saqc));
        
        builder.Services.AddSingleton<ISaqc, Saqc>();
        builder.Services.AddSingleton<IAzureStorageQueueSender, AzureStorageQueueSender>();
        builder.Services.AddHostedService<SaqcHostedService>();

        return builder;
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
        var config = new QueueConfiguration()
        {
            QueueName = queueName,
            PollingRateMs = pollingRateMs
        };
        services.AddSingleton<IQueueConfiguration>(config);
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
        var config = new QueueConfiguration()
        {
            QueueName = queueName,
            PollingRateMs = pollingRateMs
        };
        builder.Services.AddSingleton<IQueueConfiguration>(config);
        builder.Services.AddKeyedScoped<IQueueMessageHandler, THandler>(queueName);
    }
    
    /// <summary>
    /// Adds a scoped SAQC handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add.</typeparam>
    /// <param name="builder">The WebApplicationBuilder to add the handler to.</param>
    public static SaqcHandlerConfiguration<THandler> AddSaqcHandler<THandler>(this WebApplicationBuilder builder)
        where THandler : class, IQueueMessageHandler
    {
        return new SaqcHandlerConfiguration<THandler>(builder);
    }
}
