using System;
using System.Collections.Generic;

namespace Messaging.Abstractions;

/// <summary>
/// Represents message headers/attributes in a case-insensitive dictionary form.
/// Values are strings to keep provider interoperability. Complex values should be serialized by the caller.
/// </summary>
public class MessageHeaders : Dictionary<string, string?>
{
    public MessageHeaders() : base(StringComparer.OrdinalIgnoreCase) { }

    public MessageHeaders(IDictionary<string, string?> source) : base(source, StringComparer.OrdinalIgnoreCase) { }

    /// <summary>
    /// Gets a value by key if present; otherwise returns null.
    /// </summary>
    public string? Get(string key)
        => TryGetValue(key, out var v) ? v : null;

    /// <summary>
    /// Sets a header value. Null removes the key.
    /// </summary>
    public void Set(string key, string? value)
    {
        if (value is null)
            Remove(key);
        else
            this[key] = value;
    }
}
