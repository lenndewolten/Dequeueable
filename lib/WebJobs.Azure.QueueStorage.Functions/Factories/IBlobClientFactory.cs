using Azure.Core;
using Azure.Storage.Blobs;

namespace WebJobs.Azure.QueueStorage.Functions.Factories
{
    public interface IBlobClientFactory
    {
        BlobClient Create(string connectionString, string containerName, string fileName);
        BlobClient Create(Uri uri, TokenCredential tokenCredential);
    }
}