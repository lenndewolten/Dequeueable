using Azure.Storage.Blobs;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Singleton
{
    public interface IBlobClientProvider
    {
        BlobClient Get(string blobName);
    }
}