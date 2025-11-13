namespace Messaging.Abstractions.Abstractions;

public interface IMessageHandler<T>
{
    Task HandleAsync(MessageContext<T> context, CancellationToken cancellationToken = default);
}