using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleAzureQueueConsumer.Helpers;

internal static class LoggerHelper
{
    internal static void LogInfo(string message, IServiceProvider serviceProvider, bool loggingEnabled = false)
    {
        if (loggingEnabled)
        {
            Console.WriteLine(message);
        }
        else
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SaqcHostedService>>();
            logger.LogInformation(message);
        }
    }
    
    internal static void LogWarning(string message, IServiceProvider serviceProvider, bool loggingEnabled = false)
    {
        if (loggingEnabled)
        {
            Console.WriteLine(message);
        }
        else
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SaqcHostedService>>();
            logger.LogWarning(message);
        }
    }
    
    internal static void LogError(string message, IServiceProvider serviceProvider, bool loggingEnabled = false)
    {
        if (loggingEnabled)
        {
            Console.WriteLine(message);
        }
        else
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SaqcHostedService>>();
            logger.LogError(message);
        }
    }
    
    internal static void LogError(Exception ex, string message, IServiceProvider serviceProvider, bool loggingEnabled = false)
    {
        if (loggingEnabled)
        {
            Console.WriteLine(message);
        }
        else
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SaqcHostedService>>();
            logger.LogError(ex, message);
        }
    }
}