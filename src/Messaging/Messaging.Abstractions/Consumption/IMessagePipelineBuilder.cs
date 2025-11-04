using System;

namespace Messaging.Abstractions;

/// <summary>
/// Builder to compose a middleware pipeline around a terminal message handler.
/// </summary>
public interface IMessagePipelineBuilder<T>
{
    /// <summary>
    /// Adds a middleware instance to the pipeline.
    /// Middlewares are invoked in the order they are added.
    /// </summary>
    IMessagePipelineBuilder<T> Use(IMessageMiddleware<T> middleware);

    /// <summary>
    /// Builds the pipeline by wrapping the provided terminal delegate.
    /// </summary>
    MessageHandlerDelegate<T> Build(MessageHandlerDelegate<T> terminal);
}
