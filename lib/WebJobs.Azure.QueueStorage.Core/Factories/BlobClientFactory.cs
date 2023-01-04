using Azure.Core;
using Azure.Storage.Blobs;

namespace WebJobs.Azure.QueueStorage.Core.Factories
{
    internal sealed class BlobClientFactory : IBlobClientFactory
    {
        public BlobClient Create(string connectionString, string containerName, string fileName)
        {
            return new BlobClient(connectionString, containerName, fileName);
        }

        public BlobClient Create(Uri uri, TokenCredential tokenCredential)
        {
            return new BlobClient(uri, tokenCredential);
        }
    }
}
