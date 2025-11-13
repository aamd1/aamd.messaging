namespace Messaging.AzureServiceBus.Publishers;

using System.Text.Json;
using Abstractions.Abstractions;
using Azure.Messaging.ServiceBus;

internal sealed class AzureServiceBusMessagePublisher : IMessagePublisher
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AzureServiceBusMessagePublisher(ServiceBusClient client, string queueOrTopicName, string? identifier = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _sender = client.CreateSender(queueOrTopicName, new ServiceBusSenderOptions() { Identifier = identifier });
    }

    public async Task PublishAsync<T>(MessageContext<T> message, MessagePublishOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var serviceBusMessage = new ServiceBusMessage()
        {
            Body = new BinaryData(JsonSerializer.Serialize(message.Body)),
            ContentType = message.ContentType,
            MessageId = message.MessageId ?? Guid.NewGuid().ToString(),
            CorrelationId = message.CorrelationId ?? Guid.NewGuid().ToString(),
            Subject = message.Subject
        };

        if (message.Properties.Count > 0)
        {
            foreach (var messageProperty in message.Properties)
            {
                serviceBusMessage.ApplicationProperties.Add(messageProperty.Key, messageProperty.Value);
            }
        }

        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }
}