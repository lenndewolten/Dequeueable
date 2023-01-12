using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace WebJobs.Azure.QueueStorage.Functions.IntegrationTests.Fixtures
{
    public class AzuriteFixture : IAsyncLifetime
    {
        public const int BlobPort = 4000;
        public const int QueuePort = 4001;
        public const int TablePort = 4002;

        public static readonly string ConnectionString = $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:{BlobPort}/devstoreaccount1;QueueEndpoint=http://127.0.0.1:{QueuePort}/devstoreaccount1;TableEndpoint=http://127.0.0.1:{TablePort}/devstoreaccount1;";

        private readonly IDockerContainer _testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                 .WithImage("mcr.microsoft.com/azure-storage/azurite")
                 .WithPortBinding(BlobPort, 10000)
                 .WithPortBinding(QueuePort, 10001)
                 .WithPortBinding(TablePort, 10002)
                 .WithCleanUp(true)
                 .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10001))
                 .Build();

        public async Task InitializeAsync()
        {
            await _testcontainersBuilder.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _testcontainersBuilder.DisposeAsync().AsTask();
        }
    }

    [CollectionDefinition("Azurite collection")]
    public class AzuriteCollection : ICollectionFixture<AzuriteFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
