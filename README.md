# Web jobs
A framework to simplify event driven applications in containerized host environments. The aim of this project is to handle (durable) events more efficiently.

This project is inspired by the Azure Function Host. This project is an **opinionated** optimization on the Azure Function:
- Build as a Console App
- Being able to use optimized alpine/dotnet images
- Have the freedom to use Keda or any other scalers to retrieve queue messages

## Libraries
- [Functions - Queue listeners](lib/WebJobs.Azure.QueueStorage.Function/README.md)
Highly scalable queue listener that will be invoked automatically when new messages are detected on the Azure Queue.
- [Jobs - Scalable jobs](lib/WebJobs.Azure.QueueStorage.Job/README.md)
Framework that depends on external queue triggers, eg; KEDA. When the host is started, new messages on the Azure Queue are being retrieved and executed. After execution the host will shutodwn automatically.