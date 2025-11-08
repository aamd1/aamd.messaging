namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

public class RequeueDeadLetterMessagesCommandHandler
{
    private readonly ServiceBusClient _client;

    public RequeueDeadLetterMessagesCommandHandler(ServiceBusClient client)
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

        var sender = _client.CreateSender(topicName);
        int processed = 0;
        while (!cancellationToken.IsCancellationRequested && processed < maxMessages)
        {
            var remaining = maxMessages - processed;
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
                    // leave non-matching message locked to timeout; explicitly Abandon to release sooner
                    await receiver.AbandonMessageAsync(message, cancellationToken: cancellationToken);
                    continue;
                }

                Console.WriteLine($"Requeueing message {message.MessageId} {processed + 1}/{maxMessages}");
                await sender.SendMessageAsync(new ServiceBusMessage(message), cancellationToken);
                await receiver.CompleteMessageAsync(message, cancellationToken);
                processed++;
                if (processed >= maxMessages) break;
            }
        }
    }
}