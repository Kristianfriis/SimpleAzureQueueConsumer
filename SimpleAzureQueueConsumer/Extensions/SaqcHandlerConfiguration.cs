using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleAzureQueueConsumer.Interfaces;
using SimpleAzureQueueConsumer.Models;

namespace SimpleAzureQueueConsumer.Extensions;

/// <summary>
/// Configuration class for setting up a queue message handler.
/// </summary>
/// <typeparam name="THandler">The type of the queue message handler.</typeparam>
public class SaqcHandlerConfiguration<THandler> where THandler : class, IQueueMessageHandler
{
    private readonly WebApplicationBuilder _builder;
    // ReSharper disable once InconsistentNaming
    private QueueConfiguration? _queueConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaqcHandlerConfiguration{THandler}"/> class.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    public SaqcHandlerConfiguration(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Sets the queue name for the handler. <br />
    /// This is a required method to call, and should be called before configuring anything else.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> OnQueue(string queueName)
    {
        _queueConfiguration = new QueueConfiguration()
        {
            QueueName = queueName
        };
        _queueConfiguration.QueueName = queueName;

        return this;
    }

    /// <summary>
    /// Sets the polling rate for the queue. <br />
    ///
    /// Default is 5000 ms.
    /// </summary>
    /// <param name="pollingRateMs">The polling rate in milliseconds.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> WithPollingInterval(int pollingRateMs)
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.PollingRateMs = pollingRateMs;

        return this;
    }
    
    /// <summary>
    /// Sets the polling rate for the queue. <br />
    ///
    /// Default is 5 seconds.
    /// </summary>
    /// <param name="timeSpan">The polling rate in a TimeSpan.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> WithPollingInterval(TimeSpan timeSpan)
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.PollingRateMs = (int)timeSpan.TotalMilliseconds;

        return this;
    }
    
    /// <summary>
    /// Sets the visibility timeout for a dequeued message.
    /// </summary>
    /// <param name="visibilityTimeoutMs">The visibility timeout in milliseconds.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> WithVisibilityTimeout(int visibilityTimeoutMs)
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.VisibilityTimeoutMs = visibilityTimeoutMs;

        return this;
    }
    
    /// <summary>
    /// Sets the visibility timeout for a dequeued message.
    /// </summary>
    /// <param name="timeSpan">The visibility timeout in a TimeSpan.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> WithVisibilityTimeout(TimeSpan timeSpan)
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.PollingRateMs = (int)timeSpan.TotalMilliseconds;

        return this;
    }
    
    /// <summary>
    /// Sets the error queue name for the queue. <br />
    ///
    /// By default, the error queue name is the registered queue name with "-error" appended.<br />
    /// For example, if the queue name is "my-queue", the error queue name will be "my-queue-error". <br />
    /// With this you can configure a custom error queue name. It will be the full name of the error queue. <br />
    /// </summary>
    /// <param name="errorQueueName">The error queue name to send the failed messages to. Default is "-error" to registered queue name</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> WithErrorQueueName(string errorQueueName)
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.ErrorQueueName = errorQueueName;

        return this;
    }
    
    /// <summary>
    /// Configures the handler to use an error queue. <br />
    /// By default, error queues are not used. <br />
    /// </summary>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> UseErrorQueue()
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _queueConfiguration.UseErrorQueue = true;

        return this;
    }
    
    /// <summary>
    /// Registers the queue message handler and its configuration. <br />
    /// If this method is not called, the handler will not be registered.
    /// </summary>
    public void Register()
    {
        if(_queueConfiguration is null)
        {
            throw new InvalidOperationException("Queue name not set. Call OnQueue() first.");
        }
        
        _builder.Services.AddKeyedScoped<IQueueMessageHandler, THandler>(_queueConfiguration.QueueName);
        _builder.Services.AddSingleton<IQueueConfiguration>(_queueConfiguration);
    }
}