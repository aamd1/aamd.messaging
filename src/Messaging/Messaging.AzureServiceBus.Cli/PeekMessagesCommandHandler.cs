namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

public class PeekMessagesCommandHandler
{
    private readonly ServiceBusClient _client;

    public PeekMessagesCommandHandler(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(
        string topicName,
        string subscriptionName,
        bool includeDeadLettered = false,
        int maxMessages = 100,
        int maxMessagePerBatch = 100,
        int timeoutSeconds = 60,
        CancellationToken cancellationToken = default)
    {
        var receiver = _client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions
        {
            SubQueue = includeDeadLettered ? SubQueue.DeadLetter : SubQueue.None,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        if (maxMessagePerBatch > maxMessages)
        {
            maxMessagePerBatch = maxMessages;
        }

        long? sequenceNumber = null;
        int peeked = 0;
        while (!cancellationToken.IsCancellationRequested && peeked < maxMessages)
        {
            var remaining = maxMessages - peeked;
            var take = Math.Min(maxMessagePerBatch, remaining);

            IReadOnlyList<ServiceBusReceivedMessage> messages;
            if (sequenceNumber.HasValue)
            {
                messages = await receiver.PeekMessagesAsync(take, sequenceNumber.Value, cancellationToken);
            }
            else
            {
                messages = await receiver.PeekMessagesAsync(take, cancellationToken: cancellationToken);
            }

            if (messages.Count == 0)
            {
                Console.WriteLine("No more messages to peek");
                break;
            }

            foreach (var msg in messages)
            {
                Console.WriteLine($"{msg.SequenceNumber} | {msg.EnqueuedTime} | {msg.MessageId} | {msg.Subject} | DeliveryCount={msg.DeliveryCount}");
                sequenceNumber = msg.SequenceNumber + 1;
                peeked++;
                if (peeked >= maxMessages) break;
            }
        }
    }
}
