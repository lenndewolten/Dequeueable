namespace WebJobs.Azure.QueueStorage.Function.Services
{
    public interface IQueueListener
    {
        Task ListenAsync(CancellationToken cancellationToken);
    }
}