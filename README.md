# event-scaled-kubernetes-jobs
A custom implementation for Azure Functions on kubernetes. The aim of this project is to handle (durable) events more efficiently. 

The Azure Functions Host is not optimized to run in a container and/or on kubernetes. This project is an **opinionated** optimization on the Azure Function:
- Build as a Console App
- Being able to use optimized alpine/dotnet images
- Have the freedom to use Keda or any other (timed) trigger to retrieve the message

## How it works
When the (scaled) job is triggered by Keda or any other trigger, the container will retrieve the queue message and call the executer.

After execution the message will be deleted from the queue. If an exception occurres, the message will be dequeued again. If the max dequeue count is reached, the message will be moved to the poisen queue.

When the host is done handling the event, a SIGTERM event is triggered to shutdown the container/pod.


## Setup
Add a service that implements the `IQueueMessageExecutor`.
After adding the service, add `AddAzureQueueMessageService<QueueMessageExecutor>()` in the DI container of your app.

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAzureQueueMessageService<CreateGuardianEventExecutor>();
    })
    .RunConsoleAsync();
```

### Configurations
You can configure the host via the `appsettings.json` or via the `IOptions` pattern during registration.

**Appsettings**

Use the `StorageAccount` section to configure the settings:

```json
"StorageAccount": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "QueueName": "queue-name"
  }
```

**Options**

```csharp
await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAzureQueueMessageService<CreateGuardianEventExecutor>(options =>
        {
            options.AuthenticationScheme = new DefaultAzureCredential();
            options.VisibilityTimeout = TimeSpan.FromMinutes(10);
        });
    })
    .RunConsoleAsync();
```

###  settings
Setting | Description | Default | Required
--- | --- | --- | --- |
PoisenQueueSuffix | Suffix that will be used after the QueueName, eg queuename-suffix | poisen | No |
QueueName | The queue to retrieve the events from | | Yes |
ConnectionString | The connection string used to authenticate to the queue | | Yes, when not using Identiy |
AccountName | The storage account name, used for identity flow | | Only when using Identity |
StorageAccountUriFormat | The uri format to the queue storage. Used for identity flow. Use ` {accountName}` and `{queueName}` for variable substitution | https://{accountName}.queue.core.windows.net/{queueName} | No
MaxDequeueCount | Max dequeue count before moving to the poisen queue  | 5 | No |
VisibilityTimeout | The timeout after the queue message is visible again for other services | 30 seconds | No |
AuthenticationScheme | Token credential used to authenticate via AD, Any token credential provider can be used that inherits the abstract class `Azure.Core.TokenCredential` | | Yes, if you want to use Identity|

## Authentication

### SAS
Set the StorageAccountConnectionString option:

```json
"StorageAccount": {
    "ConnectionString": "UseDevelopmentStorage=true",
    ...
  }
```

### Identity
Set the `AuthenticationScheme` and the `AccountName` options to authenticate via azure AD:

```csharp
    options.AuthenticationScheme = new DefaultAzureCredential();
    options.AccountName = "devaccount" 
```

Any token credential provider can be used that inherits the abstract class `Azure.Core.TokenCredential` 


## Examples


Dependency registration in the IQueueMessageExecutor:
```csharp

internal class CreateGuardianEventExecutor : IQueueMessageExecutor
    {
        private readonly ILogger<CreateGuardianEventExecutor> _logger;

        public CreateGuardianEventExecutor(ILogger<CreateGuardianEventExecutor> logger)
        {
            _logger = logger;
        }

        public async Task Execute(QueueMessage message, CancellationToken cancellationToken)
        {
            # ... some tasks
        }
    }
```

Dockerfile - simple:
```docker
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app

COPY /samples/QueueMessages.SampleApp samples/guardians-app
COPY /src src

WORKDIR samples/guardians-app
RUN dotnet restore 
RUN dotnet publish -c Release -o /app/publish \
  --no-restore 

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "SampleApp.GuardiansOfTheGalaxy.dll"]
```


Dockerfile - advanced using alpine & non root user:
```docker
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app

COPY /samples/QueueMessages.SampleApp samples/guardians-app
COPY /src src

WORKDIR samples/guardians-app
RUN dotnet restore --runtime alpine-x64
RUN dotnet publish -c Release -o /app/publish \
  --no-restore \
  --runtime alpine-x64 \
  --self-contained true \
  /p:PublishSingleFile=true 

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS runtime
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

# upgrade musl to remove potential vulnerability
RUN apk upgrade musl
USER dotnetuser

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["./SampleApp.GuardiansOfTheGalaxy"]
```

Kubernetes deployment. This is not production safe, use secrets for the connection string!
```yaml
apiVersion: v1
kind: ConfigMap 
metadata:
  name: queuejob-consumer
data:
  StorageAccount__ConnectionString: DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://storage-azurite-service.default:10000/devstoreaccount1;QueueEndpoint=http://storage-azurite-service.default:10001/devstoreaccount1;TableEndpoint=http://storage-azurite-service.default:10002/devstoreaccount1;
  StorageAccount__QueueName: "test-queue"
---
apiVersion: keda.sh/v1alpha1
kind: ScaledJob
metadata:
  name: queuejob-consumer
  namespace: default
spec:
  jobTargetRef:
    template:
      spec:
        containers:
        - name: queuejob-executor
          image: lenndewolten/queue-event-job-executer:v1 
          imagePullPolicy: Always
          envFrom:
            - configMapRef:
                name: queuejob-consumer
        restartPolicy: Never
    backoffLimit: 4  
  pollingInterval: 10             
  maxReplicaCount: 30             
  triggers:
  - type: azure-queue
    metadata:
      queueName: test-queue
      queueLength: '1'
      connectionFromEnv: StorageAccount__ConnectionString
      accountName: devstoreaccount1
```

kubectl:

```
NAME                                          READY   STATUS      RESTARTS   AGE
queuejob-consumer-86hk2-2gx4x                 0/1     Completed   0          11s
queuejob-consumer-p4fjj-hbvjr                 0/1     Completed   0          11s
storage-azurite-deployment-67f6f9b87b-wfgmh   1/1     Running     0          4m26s
```
```
kubectl logs pods/queuejob-consumer-p4fjj-hbvjr
info: JobHandlers.AzureQueueMessage.AzureQueueMessageHostService[0]
      Azure Queue Message service started
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app
info: SampleApp.GuardiansOfTheGalaxy.Executors.CreateGuardianEventExecutor[0]
      Event '6bfd2f25-a3ea-4b86-99d9-1a2246a50560' retrieved, creating guardian
info: SampleApp.GuardiansOfTheGalaxy.Executors.CreateGuardianEventExecutor[0]
      Guardian Created!
       Name: Groot,
       Weapon: Rocket's Ultimate Blaster,
       Damage: 131.44057544725425
info: JobHandlers.AzureQueueMessage.Handlers.QueueMessageHandler[0]
      Finished executing message with id: '6bfd2f25-a3ea-4b86-99d9-1a2246a50560'
info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
info: JobHandlers.AzureQueueMessage.AzureQueueMessageHostService[0]
      Azure Queue Message service stopping
```


## Feature requests
- Handling more than one event
- Packaging
