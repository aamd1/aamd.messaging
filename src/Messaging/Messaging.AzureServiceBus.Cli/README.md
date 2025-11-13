# Messaging.AzureServiceBus.Cli

A small, cross‑platform .NET CLI for routine maintenance of Azure Service Bus topics and subscriptions, with a focus on Dead‑Letter Queue (DLQ) operations.

## What it is about
This tool helps engineers inspect and clean up messages for Azure Service Bus subscriptions. It supports:
- Peeking messages in a subscription (active or DLQ) without consuming them.
- Requeueing messages from a DLQ back to the original topic for reprocessing.
- Purging (completing) messages from a DLQ, optionally filtered by message `Subject`.

It is built on top of the official `Azure.Messaging.ServiceBus` SDK and uses the minimal Cocona command framework to provide a simple command‑line experience.

## What it does
The CLI exposes three commands:

1) `peek` — Non‑destructively view messages in a subscription, either from active messages or DLQ.
2) `requeue-dlq` — Move messages from a subscription’s DLQ back to the topic so they can be reprocessed by consumers.
3) `purge-dlq` — Permanently complete (remove) messages from a subscription’s DLQ.

Each command operates in batches, supports limits, and accepts an optional `--subject` filter to process only messages with an exact `Subject` match.

## How it works
At a glance:
- `peek` reads message headers/body without changing their state. Add `--dlq` to look in the Dead‑Letter Queue instead of the active subscription. Safe and non‑destructive.
- `requeue-dlq` takes messages from the subscription’s DLQ and sends them back to the topic so your normal consumers can try again. After sending, the DLQ copy is removed. Use this when you’ve fixed the underlying issue that caused dead‑lettering.
- `purge-dlq` permanently removes messages from the subscription’s DLQ. Use this when messages are no longer needed or should not be processed again. This action is destructive.

Filters and limits:
- `--subject` processes only messages whose `Subject` exactly matches the value you provide.
- `--max` caps how many messages the command will handle in total.
- `--batch` controls how many are handled at a time; smaller batches are slower but give you tighter control.
- `--timeout` stops the command if no messages arrive within the specified seconds.

Safety tips:
- Start with `peek` to inspect before running `requeue-dlq` or `purge-dlq`.
- Prefer using `--subject` when you want to target a specific category of messages.
- For `requeue-dlq`, downstream systems may see duplicates; ensure your handlers are idempotent.
- For `purge-dlq`, there is no undo.

## Prerequisites
- .NET SDK 8.0 or newer
- An Azure Service Bus namespace with a Topic/Subscription
- A connection string with rights to receive from subscriptions (and send to the topic for requeue)

## Build
From the solution or project directory:

```bash
# build the CLI
 dotnet build -c Release Messaging.AzureServiceBus.Cli/Messaging.AzureServiceBus.Cli.csproj

# optional: publish a self-contained exe for Windows x64
 dotnet publish Messaging.AzureServiceBus.Cli/Messaging.AzureServiceBus.Cli.csproj -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

## Usage
Each command requires at minimum `--conn`, `--topic`, and `--sub`.

Common options:
- `--conn`     Azure Service Bus connection string (required)
- `--topic`    Topic name (required)
- `--sub`      Subscription name (required)
- `--subject`  Exact `Subject` to filter messages (optional)
- `--max`      Maximum number of messages to process/peek (default: 100)
- `--batch`    Maximum messages per batch (default: 100)
- `--timeout`  Receive/peek timeout in seconds (default: 60)

### peek
Peek messages from a subscription, either active or DLQ, without consuming them.

Additional options:
- `--dlq`      When present, peek from the dead‑letter queue instead of active

Example:
```bash
# Peek up to 50 messages from DLQ
 dotnet run --project Messaging.AzureServiceBus.Cli -- peek \
   --conn "Endpoint=sb://...;SharedAccessKeyName=...;SharedAccessKey=..." \
   --topic orders --sub invoices --dlq --max 50
```

### requeue-dlq
Move messages from DLQ back to the topic for reprocessing, then complete them in DLQ.

Example:
```bash
# Requeue up to 200 messages in batches of 50, only those with Subject=Retryable
 dotnet run --project Messaging.AzureServiceBus.Cli -- requeue-dlq \
   --conn "Endpoint=sb://...;SharedAccessKeyName=...;SharedAccessKey=..." \
   --topic orders --sub invoices --subject Retryable --max 200 --batch 50
```

### purge-dlq
Complete (remove) messages from the DLQ. This is destructive.

Example:
```bash
# Purge up to 100 DLQ messages regardless of subject
 dotnet run --project Messaging.AzureServiceBus.Cli -- purge-dlq \
   --conn "Endpoint=sb://...;SharedAccessKeyName=...;SharedAccessKey=..." \
   --topic orders --sub invoices --max 100
```

## Tips and caveats
- Start with small `--max` and `--batch` when experimenting.
- Use `--subject` to limit scope when you aim to requeue/purge only specific categories.
- If no messages are received within the specified `--timeout`, the command stops with a notice.
- Requeue creates a new message from the original; make sure downstream consumers can handle duplicates if any exist out of band.

## Troubleshooting
- Authentication/Authorization: Ensure the connection string SAS policy permits `Send` (for requeue) and `Listen`/`Manage` as needed.
- Entity names: Confirm `--topic` and `--sub` exist and are correctly cased.
- Network timeouts: Increase `--timeout` or check connectivity to Azure Service Bus.
