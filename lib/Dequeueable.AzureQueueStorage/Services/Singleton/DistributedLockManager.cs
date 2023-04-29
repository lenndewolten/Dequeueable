﻿using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Dequeueable.AzureQueueStorage.Services.Singleton
{
    internal sealed class DistributedLockManager : IDistributedLockManager
    {
        private static readonly TimeSpan _leaseDuration = TimeSpan.FromSeconds(60);
        private readonly BlobClient _blobClient;
        private readonly ILogger _logger;

        public DistributedLockManager(BlobClient blobClient, ILogger logger)
        {
            _blobClient = blobClient;
            _logger = logger;
        }

        public async Task<string?> AcquireAsync(CancellationToken cancellationToken)
        {
            try
            {
                var blobProperties = await GetBlobMetadataAsync(_blobClient, cancellationToken);

                return (blobProperties?.LeaseState) switch
                {
                    null or LeaseState.Available or LeaseState.Expired or LeaseState.Broken => await TryLeaseAsync(_blobClient),
                    _ => null,
                };
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<DateTimeOffset> RenewAsync(string leaseId, CancellationToken cancellationToken)
        {
            try
            {
                var blobProperties = await GetBlobMetadataAsync(_blobClient, cancellationToken);

                if (blobProperties?.LeaseState is not LeaseState.Leased)
                {
                    throw new SingletonException($"Unable to renew the lock for {_blobClient.Name} because the lease is not active anymore");
                }

                return await TryRenewAsync(leaseId, _blobClient);
            }
            catch (RequestFailedException exception)
            {
                _logger.LogError(exception, "An error occurred while acquiring the lease for blob '{BlobName}'", _blobClient.Name);
                throw;
            }
        }

        public async Task ReleaseAsync(string leaseId, CancellationToken cancellationToken)
        {
            try
            {
                var blobLeaseClient = _blobClient.GetBlobLeaseClient(leaseId);
                // Note that this call returns without throwing if the lease is expired.
                await blobLeaseClient.ReleaseAsync(cancellationToken: cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404 || exception.Status == 409)
            {
                // if the blob no longer exists, or there is another lease
                // now active, there is nothing for us to release so we can
                // ignore
                return;
            }
        }

        private static async Task<BlobProperties?> GetBlobMetadataAsync(BlobClient blobClient, CancellationToken cancellationToken)
        {
            try
            {
                var propertiesResponse = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                return propertiesResponse.Value;
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                await TryCreateBlobAsync(blobClient, cancellationToken);
                return null;
            }
        }

        private static async Task<string> TryLeaseAsync(BlobClient blobClient)
        {
            var blobLeaseClient = blobClient.GetBlobLeaseClient();
            var leaseResponse = await blobLeaseClient.AcquireAsync(_leaseDuration);
            return leaseResponse.Value.LeaseId;
        }

        private static async Task<DateTimeOffset> TryRenewAsync(string leaseId, BlobClient blobClient)
        {
            var blobLeaseClient = blobClient.GetBlobLeaseClient(leaseId);
            await blobLeaseClient.RenewAsync();

            var nextTimeout = DateTimeOffset.UtcNow.Add(_leaseDuration);

            return nextTimeout;
        }

        private static async Task<bool> TryCreateBlobAsync(BlobClient blobClient, CancellationToken cancellationToken)
        {
            try
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty)))
                {
                    await blobClient.UploadAsync(stream, cancellationToken);
                }
                return true;
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                await TryCreateBlobContainerAsync(blobClient, cancellationToken);

                try
                {
                    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty)))
                    {
                        await blobClient.UploadAsync(stream, cancellationToken);
                    }
                    return true;
                }
                catch (RequestFailedException innerException) when (innerException.Status == 409 || innerException.Status == 412)
                {
                    // The blob already exists, or is leased by someone else
                    return false;
                }
            }
            catch (RequestFailedException exception) when (exception.Status == 409 || exception.Status == 412)
            {
                // The blob already exists, or is leased by someone else
                return false;
            }
        }

        private static async Task TryCreateBlobContainerAsync(BlobClient blobClient, CancellationToken cancellationToken)
        {
            try
            {
                var containerClient = blobClient.GetParentBlobContainerClient();
                await containerClient.CreateAsync(cancellationToken: cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 409 || exception.Status == 412)
            {
                // The container already exists
                return;
            }
        }
    }
}
