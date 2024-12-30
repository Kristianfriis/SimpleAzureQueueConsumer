using System.Text.Json;
using SimpleAzureQueueConsumer;

namespace WebApplication4.Consumers;

// Example message handler
// public class OrderCreatedMessageHandler : IQueueMessageHandler
// {
//     public Task HandleMessageAsync(StorageQueueMessage message)
//     {
//         Console.WriteLine($"Order created message received: {message.MessageText}");
//         // Process the order created message
//         var order = JsonSerializer.Deserialize<Order>(message.MessageText!);
//         Console.WriteLine(order?.UserId);
//         // ... your order processing logic ...
//         return Task.CompletedTask;
//     }
// }

public class OrderCreatedMessageHandler : IQueueMessageHandler
{
    public async Task HandleMessageAsync(StorageQueueMessage message)
    {
        // Process the order created message
        throw new Exception("An error occurred while processing the order created message");
        Console.WriteLine($"Order created: {message}");
        // ... your order processing logic ...
    }
}