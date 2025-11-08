namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

public class RequeueDeadLetterMessagesCommandHandler
{
    private readonly ServiceBusClient _client;

    public RequeueDeadLetterMessagesCommandHandler(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(string topicName, string subscriptionName, string messageSubject,
        int maxMessages = 100,
        CancellationToken cancellationToken = default)
    {
        var receiver = _client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions()
        {
            SubQueue = SubQueue.DeadLetter,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });


        var sender = _client.CreateSender(topicName);
        var receivedMessages = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var message =
                await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(60), cancellationToken);

            receivedMessages += 1;

            if (message.Subject != messageSubject)
            {
                continue;
            }

            Console.WriteLine($"Requeueing message {message.MessageId} {receivedMessages}/{maxMessages}");
            await sender.SendMessageAsync(new ServiceBusMessage(message), cancellationToken);
            await receiver.CompleteMessageAsync(message, cancellationToken);

            if (receivedMessages >= maxMessages)
            {
                break;
            }
        }
    }
}