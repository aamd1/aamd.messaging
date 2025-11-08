namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

public class PurgeDeadLetterQueueCommandHandler
{
    private readonly ServiceBusClient _client;

    public PurgeDeadLetterQueueCommandHandler(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(string topicName, string subscriptionName, string? messageSubject,
        int maxMessages = 100,
        int maxMessagePerBatch = 100,
        int timeoutSeconds = 60,
        CancellationToken cancellationToken = default)
    {
        var receiver = _client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions()
        {
            SubQueue = SubQueue.DeadLetter,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        if (maxMessagePerBatch > maxMessages)
        {
            maxMessagePerBatch = maxMessages;
        }

        int completedCount = 0;
        while (!cancellationToken.IsCancellationRequested && completedCount < maxMessages)
        {
            var remaining = maxMessages - completedCount;
            var take = Math.Min(maxMessagePerBatch, remaining);
            var messages = await receiver.ReceiveMessagesAsync(take, TimeSpan.FromSeconds(timeoutSeconds), cancellationToken);
            if (messages.Count == 0)
            {
                Console.WriteLine("No more messages in DLQ matching criteria");
                break;
            }

            foreach (var message in messages)
            {
                if (messageSubject != null && !string.Equals(message.Subject, messageSubject, StringComparison.Ordinal))
                {
                    // skip non-matching subjects
                    continue;
                }

                await receiver.CompleteMessageAsync(message, cancellationToken);
                completedCount++;
                Console.WriteLine($"Completed message {message.MessageId} {completedCount}/{maxMessages}");
                if (completedCount >= maxMessages) break;
            }
        }
    }
}