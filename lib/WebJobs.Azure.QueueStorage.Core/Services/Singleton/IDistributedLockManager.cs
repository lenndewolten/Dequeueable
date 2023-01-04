﻿namespace WebJobs.Azure.QueueStorage.Core.Services.Singleton
{
    internal interface IDistributedLockManager
    {
        Task<string?> AcquireAsync(CancellationToken cancellationToken);
        Task ReleaseAsync(string leaseId, CancellationToken cancellationToken);
        Task<DateTimeOffset> RenewAsync(string leaseId, CancellationToken cancellationToken);
    }
}