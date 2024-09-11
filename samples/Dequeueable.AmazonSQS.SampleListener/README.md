# Amazon Simple Queue Service Sample listener

## Docker

### Build

```
docker build -t <yourtagname> -f samples/Dequeueable.AmazonSQS.SampleListener/deployment/Dockerfile .
```

## Kubernetes

### Deployment

```
kubectl apply -f deployment.yaml
```

#### **Magic!**

After a message is added to the queue:

```
kubectl get pods

> NAME                                          READY   STATUS    RESTARTS       AGE
> queuelistener-deployment-75bc4b7894-gscdx    1/1     Running   0              44s
```

Logs when when four messages are handled:

```
kubectl logs pods/queuelistener-deployment-75bc4b7894-gscdx

info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app
info: Dequeueable.AmazonSQS.SampleListener.Functions.TestFunction[0]
      Function called with MessageId a365b679-eac7-4a29-b002-cd9032786a47 and content fdfdfdfdfdffdfdf
info: Dequeueable.AmazonSQS.Services.Queues.QueueMessageHandler[0]
      Executed message with id 'a365b679-eac7-4a29-b002-cd9032786a47' (Succeeded)
```
