using System;

namespace Messaging.Abstractions;

/// <summary>
/// Options that influence message publication in a transport-agnostic way.
/// Providers may map these to their native parameters when possible.
/// </summary>
public sealed class MessagePublishOptions
{
    /// <summary>
    /// Optional partition/ordering key. Transports that support ordering/partitioning may use this value.
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Optional delay before the message becomes visible/consumable.
    /// </summary>
    public TimeSpan? Delay { get; init; }

    /// <summary>
    /// Optional priority hint (higher means more priority). Not all providers support this.
    /// </summary>
    public int? Priority { get; init; }

    /// <summary>
    /// Indicates whether the message should be persisted/durable if the provider supports it. Default true.
    /// </summary>
    public bool Persistent { get; init; } = true;
}
