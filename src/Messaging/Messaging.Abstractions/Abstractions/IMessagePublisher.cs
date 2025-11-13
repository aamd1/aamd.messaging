namespace Messaging.Abstractions.Abstractions;

public interface IMessagePublisher
{
    Task PublishAsync<T>(MessageContext<T> message, MessagePublishOptions? options = null,
        CancellationToken cancellationToken = default);
}