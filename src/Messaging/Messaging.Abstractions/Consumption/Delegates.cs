using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Abstractions;

/// <summary>
/// Delegate representing the core message handling function used in the middleware pipeline.
/// </summary>
/// <typeparam name="T">Message payload type.</typeparam>
/// <param name="context">Message context.</param>
/// <param name="cancellationToken">Cancellation token.</param>
public delegate Task<MessageHandlingResult> MessageHandlerDelegate<T>(
    MessageContext<T> context,
    CancellationToken cancellationToken = default);
