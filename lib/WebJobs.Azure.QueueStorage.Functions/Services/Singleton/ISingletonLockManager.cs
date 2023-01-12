namespace WebJobs.Azure.QueueStorage.Functions.Services.Singleton
{
    public interface ISingletonLockManager
    {
        Task<string> AquireLockAsync(string fileName, CancellationToken cancellationToken);
        Task ReleaseLockAsync(string leaseId, string fileName, CancellationToken cancellationToken);
        Task<DateTimeOffset> RenewLockAsync(string leaseId, string fileName, CancellationToken cancellationToken);
    }
}