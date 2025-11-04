namespace Messaging.Abstractions;

/// <summary>
/// Abstraction to serialize/deserialize message payloads and headers.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes a payload object to a byte array.
    /// </summary>
    byte[] Serialize(object payload);

    /// <summary>
    /// Deserializes a byte array into a typed object.
    /// </summary>
    T Deserialize<T>(byte[] data);
}
