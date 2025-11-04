using System;

namespace Messaging.Abstractions;

/// <summary>
/// Standard metadata commonly carried with a message across transports.
/// </summary>
public sealed class MessageMetadata
{
    /// <summary>
    /// Globally unique message identifier. If not provided, providers may generate one.
    /// </summary>
    public string? MessageId { get; init; }

    /// <summary>
    /// Correlation id to tie a group of related messages/operations.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Causation id pointing to the parent message id that caused this message.
    /// </summary>
    public string? CausationId { get; init; }

    /// <summary>
    /// UTC time at which the message was created by the publisher.
    /// </summary>
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Logical message type (e.g., fully qualified .NET type name or domain event name).
    /// </summary>
    public string? MessageType { get; init; }

    /// <summary>
    /// Content type of the serialized payload (e.g., application/json).
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Partition/ordering key used by some brokers.
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Optional reply-to destination for RPC-like patterns.
    /// </summary>
    public string? ReplyTo { get; init; }
}
