using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Factories;

namespace WebJobs.Azure.QueueStorage.Core.Services.Singleton
{
    internal sealed class SingletonLockManager : ISingletonLockManager
    {
        private readonly ILogger<SingletonLockManager> _logger;
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly IDistributedLockManagerFactory _distributedLockManagerFactory;
        private readonly SingletonAttribute _singletonAttribute;

        public SingletonLockManager(ILogger<SingletonLockManager> logger,
        IBlobClientProvider blobClientProvider,
        IDistributedLockManagerFactory distributedLockManagerFactory,
        SingletonAttribute singletonAttribute)
        {
            _logger = logger;
            _blobClientProvider = blobClientProvider;
            _distributedLockManagerFactory = distributedLockManagerFactory;
            _singletonAttribute = singletonAttribute;
        }
        public async Task<string> AquireLockAsync(string fileName, CancellationToken cancellationToken)
        {
            var blobClient = _blobClientProvider.Get(fileName);
            var lockManager = _distributedLockManagerFactory.Create(blobClient, _logger);

            var leaseId = await AcquireLockAsync(_singletonAttribute, lockManager, cancellationToken);

            _logger.LogInformation("Lock with Id '{LeaseId}' acquired for '{FileName}'", leaseId, fileName);

            return leaseId;
        }

        public async Task<DateTimeOffset> RenewLockAsync(string leaseId, string fileName, CancellationToken cancellationToken)
        {
            var blobClient = _blobClientProvider.Get(fileName);
            var lockManager = _distributedLockManagerFactory.Create(blobClient, _logger);

            var nextVisibileOn = await lockManager.RenewAsync(leaseId, cancellationToken);

            _logger.LogInformation("Lock with Id '{LeaseId}' renewed", leaseId);
            return nextVisibileOn;
        }

        public Task ReleaseLockAsync(string leaseId, string fileName, CancellationToken cancellationToken)
        {
            var blobClient = _blobClientProvider.Get(fileName);
            var lockManager = _distributedLockManagerFactory.Create(blobClient, _logger);

            return lockManager.ReleaseAsync(leaseId, cancellationToken);
        }

        private static async Task<string> AcquireLockAsync(SingletonAttribute singleton, IDistributedLockManager lockManager, CancellationToken cancellationToken)
        {
            for (var retry = 0; retry <= singleton.MaxRetries; retry++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var leaseId = await lockManager.AcquireAsync(cancellationToken);

                if (leaseId is not null)
                {
                    return leaseId;
                }

                await Task.Delay(singleton.MinimumInterval, cancellationToken);
            }

            throw new SingletonException($"Unable to acquire lock, max retries of '{singleton.MaxRetries}' reached");
        }
    }
}
