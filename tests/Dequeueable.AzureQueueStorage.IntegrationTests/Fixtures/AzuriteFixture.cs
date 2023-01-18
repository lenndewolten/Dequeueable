using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Net.Sockets;
using System.Net;

namespace Dequeueable.AzureQueueStorage.IntegrationTests.Fixtures
{
    public class AzuriteFixture : IAsyncLifetime
    {
        private int? _blobPort;
        private int? _queuePort;
        private int? _tablePort;

        public int BlobPort
        {
            get
            {
                if (!_blobPort.HasValue)
                {
                    _blobPort = GetAvailablePort();
                }

                return _blobPort.Value;
            }
        }

        public int QueuePort
        {
            get
            {
                if (!_queuePort.HasValue)
                {
                    _queuePort = GetAvailablePort();
                }

                return _queuePort.Value;
            }
        }

        public int TablePort
        {
            get
            {
                if (!_tablePort.HasValue)
                {
                    _tablePort = GetAvailablePort();
                }

                return _tablePort.Value;
            }
        }

        public string ConnectionString => $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:{BlobPort}/devstoreaccount1;QueueEndpoint=http://127.0.0.1:{QueuePort}/devstoreaccount1;TableEndpoint=http://127.0.0.1:{TablePort}/devstoreaccount1;";

        private IDockerContainer _testcontainersBuilder => new TestcontainersBuilder<TestcontainersContainer>()
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

        private static readonly IPEndPoint _defaultLoopbackEndpoint = new(IPAddress.Loopback, port: 0);
        private static int GetAvailablePort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(_defaultLoopbackEndpoint);
                return ((IPEndPoint)socket.LocalEndPoint!).Port;
            }
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
