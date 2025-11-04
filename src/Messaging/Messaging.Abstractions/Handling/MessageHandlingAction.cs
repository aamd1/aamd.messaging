namespace Messaging.Abstractions;

/// <summary>
/// The action a subscriber should take after a handler processes a message.
/// </summary>
public enum MessageHandlingAction
{
    /// <summary>
    /// Acknowledge and remove the message from the queue/stream.
    /// </summary>
    Ack = 0,

    /// <summary>
    /// Make the message available for redelivery. Optional delay may be applied if supported.
    /// </summary>
    Retry = 1,

    /// <summary>
    /// Move the message to the dead-letter destination if configured/supported.
    /// </summary>
    DeadLetter = 2,

    /// <summary>
    /// Do nothing transport-wise (leave as-is). Use sparingly; provider behavior may vary.
    /// </summary>
    Ignore = 3
}
