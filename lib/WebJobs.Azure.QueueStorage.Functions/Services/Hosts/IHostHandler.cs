namespace WebJobs.Azure.QueueStorage.Functions.Services.Hosts
{
    public interface IHostHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}