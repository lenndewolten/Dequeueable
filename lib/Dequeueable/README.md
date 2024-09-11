# Dequeueable

Dequeueable is a flexible and extensible client library designed to simplify the process of dequeuing messages from various queuing systems. 
It abstracts common challenges such as message retrieval, exception handling, and distributed singleton patterns, enabling developers to implement robust message processing workflows across diverse environments.

- Build as a Console App
- Being able to use optimized alpine/dotnet images
- Have the freedom to use Keda or any other scalers to retrieve queue messages from various queuing systems

This framework can run as a **listener** or **job**:

- **Listener:**
  Highly scalable queue listener that will be invoked automatically when new messages are detected on the queue.
- **Job:**
  Framework that depends on external queue triggers, eg; KEDA. When the host is started, new messages on the queue are being retrieved and executed. After execution the host will shutdown automatically.

## Getting started

Scaffold a new project, you can either use a console or web app.

1. Add a Message class that implements the `IQueueMessage`.
2. Add `.AddDequeueableServices<MyMessage>` in the DI container.
3. Add the job or listener services:
   - Add `AsJob` in the DI container of your app to run the host as a job.
   - Add `AsListener` in the DI container of your app to run the app as a back ground listener.

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDequeueableServices<MyMessage>()
           .WithQueueMessageManager<QueueMessageManager>()
           .WithQueueMessageHandler<QueueMessageHandler>()
           .AsJob();
     })
    .RunConsoleAsync();
```

### Configurations
You can configure the host via the `IOptions` pattern during registration.

**Options**

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDequeueableServices<MyMessage>()
           .WithQueueMessageManager<QueueMessageManager>()
           .WithQueueMessageHandler<QueueMessageHandler>()
           .AsListener<MyOptions>();
    })
    .RunConsoleAsync();
```

### Settings

The library uses the `IOptions` pattern to inject the configured app settings. These settings will be validated on startup.

#### Listener options

| Setting                              | Description                                                                                                                    |
| ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------ |
| NewBatchThreshold                    | The threshold at which a new batch of messages will be fetched. This setting is **ignored** when using the singleton function. |
| MinimumPollingIntervalInMilliseconds | The minimum polling interval to check the queue for new messages.                                                              |
| MaximumPollingIntervalInMilliseconds | The maximum polling interval to check the queue for new messages.                                                              |
| DeltaBackOff                         | The delta used to randomize the polling interval.                                                                              |
