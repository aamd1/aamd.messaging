using System;

namespace Messaging.Abstractions;

/// <summary>
/// Options that influence subscription/consumption behavior.
/// Providers may map these to their native parameters when possible.
/// </summary>
public sealed class SubscriptionOptions
{
    /// <summary>
    /// Optional consumer group/shared subscription to coordinate multiple instances.
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// Maximum number of concurrent message handlers this subscriber should run.
    /// Providers may cap or ignore this if unsupported.
    /// </summary>
    public int? MaxConcurrentHandlers { get; init; }

    /// <summary>
    /// Prefetch/batch size hint for transports that support it.
    /// </summary>
    public ushort? PrefetchCount { get; init; }

    /// <summary>
    /// Optional dead-letter destination; used when the handler returns DeadLetter.
    /// </summary>
    public string? DeadLetterDestination { get; init; }

    /// <summary>
    /// Optional maximum delivery attempts before dead-lettering.
    /// </summary>
    public int? MaxDeliveryAttempts { get; init; }
}
