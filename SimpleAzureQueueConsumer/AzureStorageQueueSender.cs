using System.Text.Json;

namespace SimpleAzureQueueConsumer;

public class AzureStorageQueueSender : IAzureStorageQueueSender
{
    public async Task<bool> Send<T>(T message, string queueName)
    {
        var queueClient = await SaqcBase.GetOrCreateQueueClient(queueName);
        
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
}