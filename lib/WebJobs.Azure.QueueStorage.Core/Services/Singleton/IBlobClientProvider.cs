using Azure.Storage.Blobs;

namespace WebJobs.Azure.QueueStorage.Core.Services.Singleton
{
    public interface IBlobClientProvider
    {
        BlobClient Get(string blobName);
    }
}