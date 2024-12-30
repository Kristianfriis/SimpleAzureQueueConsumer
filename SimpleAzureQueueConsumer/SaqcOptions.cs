namespace SimpleAzureQueueConsumer;

public class SaqcOptions
{
    public const string Saqc = "Saqc";
    public string ConnectionString { get; set; } = string.Empty;
    public int NumberOfWorkers { get; set; } = 5;
    public bool LoggingEnabled { get; set; } = false;
}