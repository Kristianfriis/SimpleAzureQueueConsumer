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
    private readonly int _pollingRateMs = 5000;
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
    /// Sets the queue name for the handler.
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
        _queueConfiguration.PollingRateMs = _pollingRateMs;

        return this;
    }

    /// <summary>
    /// Sets the polling rate for the queue.
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
    /// Sets the polling rate for the queue.
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
    /// Sets the error queue name for the queue.
    /// </summary>
    /// <param name="errorQueueName">The error queue name appended to full queue name. Deafult is "error"</param>
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