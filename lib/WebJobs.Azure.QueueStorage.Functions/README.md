# WebJobs.Azure.QueueStorage.Job


## Getting started
Scaffold a new project, you can either use a console or web app.
1. Add a class that implements the `IAzureQueueJob`.
2. After adding the service, add `AddAzureQueueStorageJob<YourJob>` in the DI container of your app.

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAzureQueueStorageJob<TestJob>();
    })
    .RunConsoleAsync();
```

### Configurations
You can configure the host via the `appsettings.json` or via the `IOptions` pattern during registration.

**Appsettings**

Use the `WebHost` section to configure the settings:

```json
"WebHost": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "QueueName": "queue-name"
  }
```

**Options**

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAzureQueueStorageJob<TestJob>(options =>
        {
            options.AuthenticationScheme = new DefaultAzureCredential();
            options.VisibilityTimeout = TimeSpan.FromMinutes(10);
            options.QueueName = "testqueue";
        });
    })
    .RunConsoleAsync();
```

###  Settings
The library uses the `IOptions<JobHostOptions>` pattern to inject the configured app settings.

Setting | Description | Default | Required
--- | --- | --- | --- |
QueueName | The queue used to retrieve the messages. | | Yes |
ConnectionString | The connection string used to authenticate to the queue. | | Yes, when not using Azure Identity |
PoisonQueueSuffix | Suffix that will be used after the QueueName, eg queuename-suffix. | poison | No |
AccountName | The storage account name, used for identity flow. | | Only when using Identity |
QueueUriFormat | The uri format to the queue storage. Used for identity flow. Use ` {accountName}` and `{queueName}` for variable substitution. | https://{accountName}.queue.core.windows.net/{queueName} | No
AuthenticationScheme | Token credential used to authenticate via AD, Any token credential provider can be used that inherits the abstract class `Azure.Core.TokenCredential`. | | Yes, if you want to use Identity |
BatchSize | The maximum number of messages processed in parallel. | 16 | No |
MaxDequeueCount | Max dequeue count before moving to the poison queue.  | 5 | No |
VisibilityTimeoutInSeconds | The timeout after the queue message is visible again for other services.| 300 | No |
QueueClientOptions | Provides the client configuration options for connecting to Azure Queue Storage. | `new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }` | No |

## Authentication

### SAS
You can authenticate to the storage account & queue by setting the ConnectionString:

```json
"WebHost": {
    "ConnectionString": "UseDevelopmentStorage=true",
    ...
  }
```

```csharp
    services.AddAzureQueueStorageJob<TestJob>(options =>
    {
        // ...
        options.ConnectionString = "UseDevelopmentStorage=true";
    });
```

### Identity
Authenticating via Azure Identity is also possible and the recommended option. Make sure that the identity used have the following roles on the storage account
- 'Storage Queue Data Contributor'
- 'Storage Blob Data Contributor' - Only when making use of the singleton function. 

Set the `AuthenticationScheme` and the `AccountName` options to authenticate via azure AD:

```csharp
    services.AddAzureQueueStorageJob<TestJob>(options =>
    {
        options.AuthenticationScheme = new DefaultAzureCredential();
        options.AccountName = "thestorageaccountName";
    });
```
Any token credential provider can be used that inherits the abstract class `Azure.Core.TokenCredential` 

The `QueueUriFormat` options is used to format the correct URI to the queue. When making use of the singleton function, the `BlobUriFormat` is used to format the correct URI to the blob lease.

### Custom QueueProvider
There are plenty ways to construct the QueueClient, and not all are by default supported. You can override the default implementations to retrieve the queue client by implementing the `IQueueClientProvider`. You still should register your custom provider in your DI container, specific registration order is not needed: 

```csharp
internal class MyCustomQueueProvider : IQueueClientProvider
    {
        public QueueClient GetQueue()
        {
            return new QueueClient(new Uri("https://myaccount.chinacloudapi.cn/myqueue"), new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
        }

        public QueueClient GetPoisonQueue()
        {
            return new QueueClient(new Uri("https://myaccount.chinacloudapi.cn/mypoisonqueue"), new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
        }
    }
```

## Singleton
A singleton attribute can be applied the job to ensure that only a single  instance of the job is executed at any given time. It uses the blob lease and therefore **distributed** lock is guaranteed. The blob is always leased for 60 seconds. The lease will be released if no longer required. It will be automatically renewed if executing the message(s) takes longer.

NOTE: The blob files will not be automatically deleted. If needed, consider specifying data lifecycle rules for the blob container: https://learn.microsoft.com/en-us/azure/storage/blobs/lifecycle-management-overview

Set the `Singleton("<scope>"` attribute above the job:

```csharp
    [Singleton("Id")]
    internal class SampleSingletonJob : IAzureQueueJob
    {
        //...
    }
```

Only messages containing a JSON format is supported. The scope should **always** be a property in the message body that exists.

Given a queue message with the following body:
```json
{
    "Id": "d89c209a-6b81-4266-a768-8cde6f613753",
    // ...
}
```
When the scope is set to `[Singleton("Id")]` on the job. Only a single message containing id "d89c209a-6b81-4266-a768-8cde6f613753" will be executed at an given time.

Nested properties are also supported. Given a queue message with the following body:
```json
{
    "My": {
        "Nested": {
            "Property": 500
        }
    }
    // ...
}
```
When the scope is set to `[Singleton("My:Nested:Property")]` on the function. Only a single message containing `500` will be executed at an given time.

### Singleton Options
You can specify the following singleton options via the singleton attribute `[Singleton(scope: "Id", containerName: ContainerName, minimumIntervalInSeconds: 1)]`:

Setting | Description | Default | Required
--- | --- | --- | --- |
MinimumIntervalInSeconds | The minimum polling interval to check if a new lease can be acquired.  | 10 | No |
MaximumIntervalInSeconds | The maximum polling interval to check if a new lease can be acquired.  | 120 | No |
MaxRetries | The max retries to acquire a lease. | 3 | No |
ContainerName | The container name for the lock files. | webjobshost | No |
BlobUriFormat | The uri format to the blob storage. Used for identity flow. Use ` {accountName}`, `{containerName}` and `{blobName}` for variable substitution. | "https://{accountName}.blob.core.windows.net/{containerName}/{blobName}" | No

### Custom BlobClientProvider
There are plenty ways to construct the BlobClient, and not all are by default supported. You can override the default implementations to retrieve the blob client for the lease by implementing the `IBlobClientProvider`. You still should register your custom provider in your DI container, specific registration order is not needed: 

```csharp
internal class MyCustomBlobClientProvider : IBlobClientProvider
    {
        public BlobClient Get(string blobName)
        {
            return new BlobClient(new Uri($"https://myaccount.chinacloudapi.cn/mycontainer/{blobName}"), 
                new BlobClientOptions { GeoRedundantSecondaryUri = new Uri($"https://mysecaccount.chinacloudapi.cn/mycontainer/{blobName}") });
        }
    }
```

## Timeouts

### Visibility Timeout Queue Message
The visibility timeout of the queue messages is automatically updated. It will be updated when the half `VisibilityTimeout` option is reached. Choose this setting wisely to prevent talkative hosts. When renewing the timeout fails, the host cannot guarantee if the message is executed only once. Therefore the CancelationToken is set to Cancelled. It is up to you how to handle this scenario!

### Lease timeout
The lease timeout of the blob lease is automatically updated. It will be updated when the half lease is reached. When renewing the timeout fails, the host cannot guarantee the lock. Therefore the CancelationToken is set to Cancelled. It is up to you how to handle this scenario!


## Sample
- [Console app](../../samples/WebJobs.Azure.QueueStorage.Job.SampleConsoleApp/README.md)