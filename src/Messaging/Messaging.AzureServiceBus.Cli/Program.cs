namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;
using Cocona;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = CoconaApp.CreateBuilder();
            var app = builder.Build();

            app.AddCommand("purge-dlq", async (
                [Option("conn", Description = "Service Bus connection string")] string? conn,
                [Option("topic", Description = "Topic name")] string? topic,
                [Option("sub", Description = "Subscription name")] string? sub,
                [Option("subject", Description = "Filter messages by exact Subject")] string? subject,
                [Option("max", Description = "Max messages to process")] int max = 100,
                [Option("batch", Description = "Max messages per batch")] int batch = 100,
                [Option("timeout", Description = "Receive timeout in seconds")] int timeout = 60,
                CancellationToken cancellationToken = default) =>
            {
                if (string.IsNullOrWhiteSpace(conn) || string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(sub))
                {
                    Console.Error.WriteLine("Required options: --conn, --topic, --sub");
                    return;
                }
                await using var client = new ServiceBusClient(conn);
                var handler = new PurgeDeadLetterQueueCommandHandler(client);
                await handler.HandleAsync(topic!, sub!, subject, max, batch, timeout, cancellationToken);
            })
            .WithDescription("Purge (complete) messages from the dead-letter queue matching optional criteria");

            app.AddCommand("requeue-dl", async (
                [Option("conn", Description = "Service Bus connection string")] string? conn,
                [Option("topic", Description = "Topic name")] string? topic,
                [Option("sub", Description = "Subscription name")] string? sub,
                [Option("subject", Description = "Only requeue messages with this exact Subject")] string? subject,
                [Option("max", Description = "Max messages to process")] int max = 100,
                [Option("batch", Description = "Max messages per batch")] int batch = 100,
                [Option("timeout", Description = "Receive timeout in seconds")] int timeout = 60,
                CancellationToken cancellationToken = default) =>
            {
                if (string.IsNullOrWhiteSpace(conn) || string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(sub))
                {
                    Console.Error.WriteLine("Required options: --conn, --topic, --sub");
                    return;
                }
                await using var client = new ServiceBusClient(conn);
                var handler = new RequeueDeadLetterMessagesCommandHandler(client);
                await handler.HandleAsync(topic, sub, subject, max, batch, timeout, cancellationToken);
            })
            .WithDescription("Requeue messages from the dead-letter queue back to the topic");

            app.AddCommand("peek", async (
                [Option("conn", Description = "Service Bus connection string")] string? conn,
                [Option("topic", Description = "Topic name")] string? topic,
                [Option("sub", Description = "Subscription name")] string? sub,
                [Option("dlq", Description = "Peek from the dead-letter queue instead of active")] bool dlq = false,
                [Option("max", Description = "Max messages to peek")] int max = 100,
                [Option("batch", Description = "Max messages per peek batch")] int batch = 100,
                [Option("timeout", Description = "Peek timeout in seconds")] int timeout = 60,
                CancellationToken cancellationToken = default) =>
            {
                if (string.IsNullOrWhiteSpace(conn) || string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(sub))
                {
                    Console.Error.WriteLine("Required options: --conn, --topic, --sub");
                    return;
                }
                await using var client = new ServiceBusClient(conn);
                var handler = new PeekMessagesCommandHandler(client);
                await handler.HandleAsync(topic, sub, dlq, max, batch, timeout, cancellationToken);
            })
            .WithDescription("Peek messages from a subscription (active or DLQ)");

            await app.RunAsync();
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return 10;
        }
    }
}