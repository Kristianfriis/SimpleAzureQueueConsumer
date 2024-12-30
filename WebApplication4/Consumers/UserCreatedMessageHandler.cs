using System.Text.Json;
using SimpleAzureQueueConsumer.Interfaces;
using SimpleAzureQueueConsumer.Models;

namespace WebApplication4.Consumers;

public class UserCreatedMessageHandler : IQueueMessageHandler
{
    private readonly IAzureStorageQueueSender _azureStorageQueueSender;

    public UserCreatedMessageHandler(IAzureStorageQueueSender azureStorageQueueSender)
    {
        _azureStorageQueueSender = azureStorageQueueSender;
    }

    public async Task HandleMessageAsync(StorageQueueMessage message)
    {
        var user = JsonSerializer.Deserialize<User>(message.MessageText!);
        Console.WriteLine($"User created message received: user id: {user?.Email}");
        // ... your order processing logic ...
        await _azureStorageQueueSender.Send(new Order(){UserId = user.Id}, "order-created");
    }
}