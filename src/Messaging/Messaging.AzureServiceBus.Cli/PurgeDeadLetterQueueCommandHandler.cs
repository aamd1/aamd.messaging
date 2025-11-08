namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

public class PurgeDeadLetterQueueCommandHandler
{
    private readonly ServiceBusClient _client;

    public PurgeDeadLetterQueueCommandHandler(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(string topicName, string subscriptionName, string messageSubject,
        int maxMessages = 100,
        int maxMessagePerBatch = 100, CancellationToken cancellationToken = default)
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

        int receivedMessages = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var messages =
                await receiver.ReceiveMessagesAsync(maxMessagePerBatch, TimeSpan.FromSeconds(60),
                    CancellationToken.None);
            if (messages.Count == 0)
            {
                Console.WriteLine("No more messages");
                break;
            }

            receivedMessages += messages.Count;

            foreach (var message in messages)
            {
                if (message.Subject != messageSubject)
                {
                    continue;
                }

                await receiver.CompleteMessageAsync(message, cancellationToken);
                Console.WriteLine($"Completed message {message.MessageId} {receivedMessages}/{maxMessages}");
            }

            if (receivedMessages >= maxMessages)
            {
                Console.WriteLine($"Reached max messages of {maxMessages}, stopping");
                break;
            }
        }
    }
}