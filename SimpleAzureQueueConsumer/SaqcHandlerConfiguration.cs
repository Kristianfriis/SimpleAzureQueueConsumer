using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleAzureQueueConsumer;

/// <summary>
/// Configuration class for setting up a queue message handler.
/// </summary>
/// <typeparam name="THandler">The type of the queue message handler.</typeparam>
public class SaqcHandlerConfiguration<THandler> where THandler : class, IQueueMessageHandler
{
    private readonly WebApplicationBuilder _builder;
    private readonly int _pollingRateMs = 5000;
    private QueueConfiguration _queueConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaqcHandlerConfiguration{THandler}"/> class.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    public SaqcHandlerConfiguration(WebApplicationBuilder builder)
    {
        _builder = builder;
        _queueConfiguration = new QueueConfiguration();
    }

    /// <summary>
    /// Sets the queue name for the handler.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> OnQueue(string queueName)
    {
        _queueConfiguration.QueueName = queueName;
        _queueConfiguration.PollingRateMs = _pollingRateMs;

        return this;
    }

    /// <summary>
    /// Sets the polling rate for the queue.
    /// </summary>
    /// <param name="pollingRateMs">The polling rate in milliseconds.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> SetPollingRate(int pollingRateMs)
    {
        _queueConfiguration.PollingRateMs = pollingRateMs;

        return this;
    }
    
    /// <summary>
    /// Sets the polling rate for the queue.
    /// </summary>
    /// <param name="timeSpan">The polling rate in a TimeSpan.</param>
    /// <returns>The current instance of <see cref="SaqcHandlerConfiguration{THandler}"/>.</returns>
    public SaqcHandlerConfiguration<THandler> SetPollingRate(TimeSpan timeSpan)
    {
        _queueConfiguration.PollingRateMs = (int)timeSpan.TotalMilliseconds;

        return this;
    }

    /// <summary>
    /// Registers the queue message handler and its configuration.
    /// </summary>
    public void Register()
    {
        _builder.Services.AddKeyedScoped<IQueueMessageHandler, THandler>(_queueConfiguration.QueueName);
        SaqcBase.AddQueueConfig(_queueConfiguration);
    }
}