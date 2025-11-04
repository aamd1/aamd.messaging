namespace Messaging.Abstractions;

/// <summary>
/// Provider-agnostic contract to publish messages.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message payload to a destination (topic/queue/exchange) with optional headers and options.
    /// </summary>
    /// <param name="destination">Logical destination name (topic, queue, subject, etc.).</param>
    /// <param name="payload">Message payload object that will be serialized by an <see cref="IMessageSerializer"/>.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="options">Provider-specific optional hints such as partition key, delay, etc.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync(
        string destination,
        object payload,
        MessageHeaders? headers = null,
        MessagePublishOptions? options = null,
        CancellationToken cancellationToken = default);
}
