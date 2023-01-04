# WebJobs.Azure.QueueStorage.Job.SampleConsoleApp


## Docker

### Build
```
docker build -t <yourtagname> -f samples/WebJobs.Azure.QueueStorage.Job.SampleConsoleApp/deployment/Dockerfile .
```
Image stats:
```
docker images -f reference=lenndewolten/webjobs
> REPOSITORY             TAG                    IMAGE ID       CREATED          SIZE
> lenndewolten/webjobs   samplejob-v1           fe54c3e50dcf   45 minutes ago   80.7MB
```

```
docker scan lenndewolten/webjobs:samplejob-v1  

> Testing lenndewolten/webjobs:samplejob-v1...
> 
> Package manager:   apk
> Project name:      docker-image|lenndewolten/webjobs
> Docker image:      lenndewolten/webjobs:samplejob-v1
> Platform:          linux/amd64
> Base image:        alpine:3.16.3
> 
> ✔ Tested 23 dependencies for known vulnerabilities, no vulnerable paths found.
```


## Kubernetes

### Deployment
This sample is using [KEDA](https://keda.sh/) to automatically schedule the jobs based on the messages on the queue

```
kubectl apply -f azurite.yaml
kubectl apply -f scaledjob.yaml
```

#### **Connect to azurite**
Get the public IP address of one of your nodes that is running a Hello World pod. How you get this address depends on how you set up your cluster. For example, if you are using Minikube or Docker Desktop, you can see the node address by running kubectl cluster-info. 

```
kubectl cluster-info

> Kubernetes control plane is running at https://kubernetes.docker.internal:6443
> CoreDNS is running at https://kubernetes.docker.internal:6443/api/v1/namespaces/kube-system/services/kube-dns:dns/proxy
```

####  **Get Azurite NodePort IP**

```
 kubectl get svc

> NAME                      TYPE        CLUSTER-IP      EXTERNAL-IP   PORT> (S)                                           AGE
> kubernetes                ClusterIP   10.96.0.1       <none>        443/> TCP                                           209d
> storage-azurite-service   NodePort    10.104.26.110   <none>        10000:32318/TCP,10001:30528/TCP,> 10002:30802/TCP   208d
```

####  **Construct connection string**
With the output above, the connection string would be:
```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://kubernetes.docker.internal:32318/devstoreaccount1;QueueEndpoint=http://kubernetes.docker.internal:30528/devstoreaccount1;TableEndpoint=http://kubernetes.docker.internal:30802/devstoreaccount1;
```

####  **Magic!**
After a message is added to the queue:
```
kubectl get pods

> NAME                                          READY   STATUS    RESTARTS       AGE
> queuejob-consumer-hqrpw-bwjr5                 1/1     Running   0              7s
> storage-azurite-deployment-67f6f9b87b-wfgmh   1/1     Running   56 (61m ago)   197d
```

```
kubectl get pods

> NAME                                          READY   STATUS      RESTARTS       AGE
> queuejob-consumer-hqrpw-bwjr5                 0/1     Completed   0              2m51s
> storage-azurite-deployment-67f6f9b87b-wfgmh   1/1     Running     56 (64m ago)   197d
```

Logs when when four messages are handled:
```
kubectl logs pods/queuejob-consumer-rs8t6-gjrdl

> info: Microsoft.Hosting.Lifetime[0]
>       Application started. Press Ctrl+C to shut down.
> info: Microsoft.Hosting.Lifetime[0]
>       Hosting environment: Production
> info: Microsoft.Hosting.Lifetime[0]
>       Content root path: /app
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 0
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 0
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 0
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 0
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 1
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 1
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 1
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 1
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 2
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 2
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 2
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 2
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 3
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 3
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 3
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 3
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 4
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 4
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 4
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 4
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 5
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 5
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 5
> info: WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job.TestJob[0]
>       Executing job loop 5
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'c134e005-7f93-4415-b979-5e388771510b' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'fe0169f5-9a13-425e-bb42-fc4946774ab6' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'e370feff-855e-4f7c-8e0e-8f0c92170013' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id '45f69fe7-04b3-44ee-a110-816c21b60bce' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'c134e005-7f93-4415-b979-5e388771510b' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'e370feff-855e-4f7c-8e0e-8f0c92170013' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id '45f69fe7-04b3-44ee-a110-816c21b60bce' (Succeeded)
> info: WebJobs.Azure.QueueStorage.Core.Services.Queues.QueueMessageHandler[0]
>       Executed message with id 'fe0169f5-9a13-425e-bb42-fc4946774ab6' (Succeeded)
> info: Microsoft.Hosting.Lifetime[0]
>       Application is shutting down...
```
