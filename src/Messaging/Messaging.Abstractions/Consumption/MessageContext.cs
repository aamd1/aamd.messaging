using System;
using System.Collections.Generic;

namespace Messaging.Abstractions;

/// <summary>
/// Context for a received message, including envelope and transport-specific details projected into common fields.
/// </summary>
public sealed class MessageContext<T>
{
    public MessageContext(
        MessageEnvelope<T> envelope,
        string destination,
        int deliveryAttempt = 1,
        DateTimeOffset? receivedAtUtc = null)
    {
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        DeliveryAttempt = deliveryAttempt > 0 ? deliveryAttempt : 1;
        ReceivedAtUtc = receivedAtUtc ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// The received message envelope.
    /// </summary>
    public MessageEnvelope<T> Envelope { get; }

    /// <summary>
    /// The logical destination (topic/queue) this message was consumed from.
    /// </summary>
    public string Destination { get; }

    /// <summary>
    /// The number of times this message has been delivered (1 for first attempt).
    /// </summary>
    public int DeliveryAttempt { get; }

    /// <summary>
    /// UTC timestamp when the message was received by the subscriber.
    /// </summary>
    public DateTimeOffset ReceivedAtUtc { get; }

    /// <summary>
    /// A per-message bag to share data across middleware/handlers.
    /// </summary>
    public IDictionary<string, object?> Items { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);
}
