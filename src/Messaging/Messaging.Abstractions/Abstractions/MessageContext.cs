namespace Messaging.Abstractions.Abstractions;

public class MessageContext<T>
{
    public string? CorrelationId { get; set; }
    public string? MessageId { get; set; } = Guid.NewGuid().ToString();
    public string? Subject { get; set; }
    public DateTime EnqueuedTimeUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public string Version { get; set; } = "1.0";
    public string ContentType { get; set; } = "application/json";

    public Dictionary<string, object> Properties { get; set; } =
        new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    public T? Body { get; set; }
}