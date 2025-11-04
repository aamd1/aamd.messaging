using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Abstractions;

/// <summary>
/// Typed message handler invoked by the subscriber pipeline.
/// </summary>
/// <typeparam name="T">Message payload type.</typeparam>
public interface IMessageHandler<T>
{
    /// <summary>
    /// Handles a single message from a subscription.
    /// Return a <see cref="MessageHandlingResult"/> to indicate ack/retry/dead-letter.
    /// </summary>
    Task<MessageHandlingResult> HandleAsync(MessageContext<T> context, CancellationToken cancellationToken = default);
}
