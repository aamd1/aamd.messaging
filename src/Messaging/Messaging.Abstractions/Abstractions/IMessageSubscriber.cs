using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Abstractions;

/// <summary>
/// Provider-agnostic contract to subscribe to messages from a destination and dispatch them to handlers.
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Subscribes to a destination and delivers messages of type <typeparamref name="T"/> to the specified handler delegate.
    /// Returns an <see cref="ISubscription"/> that can be disposed/Stopped to cancel the subscription.
    /// </summary>
    Task<ISubscription> SubscribeAsync<T>(
        string destination,
        MessageHandlerDelegate<T> handler,
        SubscriptionOptions? options = null,
        CancellationToken cancellationToken = default);
}
