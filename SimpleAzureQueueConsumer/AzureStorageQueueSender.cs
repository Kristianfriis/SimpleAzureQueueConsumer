using System.Collections.Concurrent;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using SimpleAzureQueueConsumer.Interfaces;

namespace SimpleAzureQueueConsumer;

/// <summary>
/// Service for sending messages to Azure Storage Queues.
/// </summary>
public class AzureStorageQueueSender : IAzureStorageQueueSender
{
    private SaqcOptions _options;
    private ConcurrentDictionary<string, QueueClient>? QueueClients { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureStorageQueueSender"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the queue sender.</param>
    public AzureStorageQueueSender(IOptions<SaqcOptions> options)
    {
        _options = options.Value;
        QueueClients = new ();
    }

    /// <summary>
    /// Sends a message to the specified queue.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="queueName">The name of the queue.</param>
    /// <returns>A task that represents the asynchronous send operation. The task result contains a boolean indicating whether the message was sent successfully.</returns>
    public async Task<bool> Send<T>(T message, string queueName)
    {
        var queueClient = await GetOrCreateQueueClient(queueName);

        var json = JsonSerializer.Serialize(message);

        try
        {
            await queueClient.SendMessageAsync(json);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// Gets or creates a <see cref="QueueClient"/> for the specified queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="QueueClient"/> for the specified queue.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the queue clients dictionary is not initialized.</exception>
    private async Task<QueueClient> GetOrCreateQueueClient(string queueName)
    {
        if(QueueClients is null)
        {
            //TODO User defined exception
            throw new InvalidOperationException("Queue clients not initialized.");
        }

        if (QueueClients.TryGetValue(queueName, out var queueClient))
        {
            return queueClient;
        }

        queueClient = new QueueClient(_options.ConnectionString, queueName);
        await queueClient.CreateIfNotExistsAsync();
        QueueClients.TryAdd(queueName, queueClient);
        return queueClient;
    }
}