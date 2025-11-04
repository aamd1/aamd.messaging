using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Abstractions;

/// <summary>
/// Represents an active subscription that can be stopped and disposed.
/// Implementations should be thread-safe.
/// </summary>
public interface ISubscription : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Logical destination this subscription is consuming from.
    /// </summary>
    string Destination { get; }

    /// <summary>
    /// Stops the subscription. Multiple calls should be safe (idempotent).
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
