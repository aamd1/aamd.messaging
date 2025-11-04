using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Abstractions;

/// <summary>
/// Middleware that can wrap message handling with cross-cutting concerns (logging, retries, metrics, etc.).
/// </summary>
public interface IMessageMiddleware<T>
{
    Task<MessageHandlingResult> InvokeAsync(
        MessageContext<T> context,
        MessageHandlerDelegate<T> next,
        CancellationToken cancellationToken = default);
}
