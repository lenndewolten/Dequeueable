namespace WebJobs.Azure.QueueStorage.Job.Services
{
    public interface IQueueMessagesHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}