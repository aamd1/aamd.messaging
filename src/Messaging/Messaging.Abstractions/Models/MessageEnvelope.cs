using System;

namespace Messaging.Abstractions;

/// <summary>
/// Envelope of a message with strongly-typed body and associated headers/metadata.
/// </summary>
public sealed class MessageEnvelope<T>
{
    public MessageEnvelope(T body, MessageHeaders? headers = null, MessageMetadata? metadata = null)
    {
        Body = body ?? throw new ArgumentNullException(nameof(body));
        Headers = headers ?? new MessageHeaders();
        Metadata = metadata ?? new MessageMetadata();
    }

    /// <summary>
    /// The message payload.
    /// </summary>
    public T Body { get; }

    /// <summary>
    /// Message headers.
    /// </summary>
    public MessageHeaders Headers { get; }

    /// <summary>
    /// Message metadata.
    /// </summary>
    public MessageMetadata Metadata { get; }
}
