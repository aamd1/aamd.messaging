using System;

namespace Messaging.Abstractions;

/// <summary>
/// Result returned by a message handler to instruct the subscriber what to do with the message.
/// </summary>
public sealed class MessageHandlingResult
{
    private MessageHandlingResult(MessageHandlingAction action, TimeSpan? retryDelay = null, string? reason = null, Exception? exception = null)
    {
        Action = action;
        RetryDelay = retryDelay;
        Reason = reason;
        Exception = exception;
    }

    public MessageHandlingAction Action { get; }

    /// <summary>
    /// Optional retry delay. Used only when <see cref="Action"/> is <see cref="MessageHandlingAction.Retry"/>.
    /// </summary>
    public TimeSpan? RetryDelay { get; }

    /// <summary>
    /// Optional human-readable reason.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Optional exception captured by the handler.
    /// </summary>
    public Exception? Exception { get; }

    public static MessageHandlingResult Ack(string? reason = null) => new(MessageHandlingAction.Ack, null, reason);
    public static MessageHandlingResult Retry(TimeSpan? delay = null, string? reason = null, Exception? ex = null) => new(MessageHandlingAction.Retry, delay, reason, ex);
    public static MessageHandlingResult DeadLetter(string? reason = null, Exception? ex = null) => new(MessageHandlingAction.DeadLetter, null, reason, ex);
    public static MessageHandlingResult Ignore(string? reason = null) => new(MessageHandlingAction.Ignore, null, reason);
}
