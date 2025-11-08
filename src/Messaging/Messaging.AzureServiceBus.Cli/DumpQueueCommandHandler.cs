namespace Messaging.AzureServiceBus.Cli;

using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Messaging.ServiceBus;

public record MessageItem
{
    public string Id { get; set; }
}

public record MessageData
{
    public List<MessageItem> Consumers { get; set; }
}

public record MessageEnvelope
{
    public MessageData Data { get; set; }
}

public class DumpQueueCommandHandler
{
    private readonly ServiceBusClient _client;

    public DumpQueueCommandHandler(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(string topicName, string subscriptionName, bool includeDeadLettered = false,
        int timeoutSeconds = 60,
        int maxMessages = 100, int maxMessagePerBatch = 100, string? destinationPath = null,
        string pattern = @"""id"":\s*""(\d+)""",
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Dumping {maxMessages}, by {maxMessagePerBatch} per batch from {topicName}/{subscriptionName} continue ?");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        
        var receiver = _client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions()
        {
            SubQueue = includeDeadLettered ? SubQueue.DeadLetter : SubQueue.None,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        if (destinationPath == null)
        {
            destinationPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.json");
        }

        if (maxMessagePerBatch > maxMessages)
        {
            maxMessagePerBatch = maxMessages;
        }

        int messagesReceived = 0;
        await using FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
        await using Utf8JsonWriter writer = new Utf8JsonWriter(fs, new JsonWriterOptions
        {
            Indented = true
        });
        writer.WriteStartArray();
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var messages =
                    await receiver.ReceiveMessagesAsync(maxMessagePerBatch, TimeSpan.FromSeconds(timeoutSeconds),
                        cancellationToken);
                messagesReceived += messages.Count;
                if (messages.Count == 0)
                {
                    Console.WriteLine("No more messages");
                    break;
                }

                foreach (var message in messages)
                {
                    Console.WriteLine($"Received message: {message.MessageId}, {message.Subject}");
                    Console.WriteLine($"Processing message {message.MessageId}");
                    if (message.Subject != "CesuConsumersChanged")
                    {
                        continue;
                    }

                    var envelope = JsonSerializer.Deserialize<MessageEnvelope>(message.Body.ToString());

                    foreach (var consumer in envelope.Data.Consumers)
                    {
                        writer.WriteStringValue(consumer.Id);
                    }
                    
                    Console.WriteLine("Processed message {0}/{1} - {2} consumers", messagesReceived, maxMessages, envelope.Data.Consumers.Count);
                }

                if (messagesReceived >= maxMessages)
                {
                    Console.WriteLine("Reached max messages, stopping");
                    break;
                }
            }
        }
        finally
        {
            Console.WriteLine($"Received {messagesReceived} messages");
            Console.WriteLine($"Saved hits to {destinationPath}");
            writer.WriteEndArray();
        }
    }
}