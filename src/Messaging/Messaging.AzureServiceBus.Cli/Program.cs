namespace Messaging.AzureServiceBus.Cli;

using Azure.Messaging.ServiceBus;

class Program
{
    private static int ParseInt(IDictionary<string, string> args, string key, int defaultValue)
    {
        return args.TryGetValue(key, out var v) && int.TryParse(v, out var i) ? i : defaultValue;
    }

    private static bool HasFlag(IDictionary<string, string> args, string key)
    {
        return args.ContainsKey(key);
    }

    private static IDictionary<string, string> ParseArgs(string[] argv)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < argv.Length; i++)
        {
            var token = argv[i];
            if (token.StartsWith("--"))
            {
                var key = token.Substring(2);
                if (i + 1 < argv.Length && !argv[i + 1].StartsWith("--"))
                {
                    dict[key] = argv[++i];
                }
                else
                {
                    dict[key] = "true"; // flag
                }
            }
            else if (!dict.ContainsKey("_cmd"))
            {
                dict["_cmd"] = token;
            }
        }
        return dict;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Azure Service Bus CLI");
        Console.WriteLine("Usage:");
        Console.WriteLine("  asb purge-dlq --conn <connection> --topic <name> --sub <name> [--subject <text>] [--max <n>] [--batch <n>] [--timeout <s>]");
        Console.WriteLine("  asb requeue-dl --conn <connection> --topic <name> --sub <name> [--subject <text>] [--max <n>] [--batch <n>] [--timeout <s>]");
        Console.WriteLine("  asb peek --conn <connection> --topic <name> --sub <name> [--dlq] [--max <n>] [--batch <n>] [--timeout <s>]");
        Console.WriteLine("  asb dump --conn <connection> --topic <name> --sub <name> [--dlq] [--subject <text>] [--max <n>] [--batch <n>] [--timeout <s>] [--dest <path>]");
        Console.WriteLine();
    }

    static async Task<int> Main(string[] argv)
    {
        try
        {
            if (argv.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            var parsed = ParseArgs(argv);
            if (!parsed.TryGetValue("_cmd", out var cmd))
            {
                Console.Error.WriteLine("Missing command");
                PrintUsage();
                return 1;
            }

            if (!parsed.TryGetValue("conn", out var conn))
            {
                Console.Error.WriteLine("Missing required option: --conn");
                return 2;
            }

            if (!parsed.TryGetValue("topic", out var topic))
            {
                Console.Error.WriteLine("Missing required option: --topic");
                return 3;
            }

            if (!parsed.TryGetValue("sub", out var sub))
            {
                Console.Error.WriteLine("Missing required option: --sub");
                return 4;
            }

            var subject = parsed.TryGetValue("subject", out var subj) ? subj : null;
            var max = ParseInt(parsed, "max", 100);
            var batch = ParseInt(parsed, "batch", Math.Min(100, max));
            var timeout = ParseInt(parsed, "timeout", 60);
            var useDlq = HasFlag(parsed, "dlq");
            var dest = parsed.TryGetValue("dest", out var d) ? d : null;

            await using var client = new ServiceBusClient(conn);

            switch (cmd.ToLowerInvariant())
            {
                case "purge-dlq":
                {
                    var handler = new PurgeDeadLetterQueueCommandHandler(client);
                    await handler.HandleAsync(topic, sub, subject, max, batch, timeout, CancellationToken.None);
                    break;
                }
                case "requeue-dl":
                {
                    var handler = new RequeueDeadLetterMessagesCommandHandler(client);
                    await handler.HandleAsync(topic, sub, subject, max, batch, timeout, CancellationToken.None);
                    break;
                }
                case "peek":
                {
                    var handler = new PeekMessagesCommandHandler(client);
                    await handler.HandleAsync(topic, sub, useDlq, max, batch, timeout, CancellationToken.None);
                    break;
                }
                default:
                    Console.Error.WriteLine($"Unknown command: {cmd}");
                    PrintUsage();
                    return 5;
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return 10;
        }
    }
}