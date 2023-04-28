using System.Net.Sockets;
using System.Net;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;

namespace Dequeueable.AmazonSQS.IntegrationTests.Fixtures
{
    public class LocalStackFixture : IAsyncLifetime
    {
        private int? _port;

        public int Port
        {
            get
            {
                if (!_port.HasValue)
                {
                    _port = GetAvailablePort();
                }

                return _port.Value;
            }
        }

        public string SQSURL => $"http://localhost:{Port}";

        private IDockerContainer _testcontainersBuilder => new TestcontainersBuilder<TestcontainersContainer>()
                 .WithImage("localstack/localstack")
                 .WithPortBinding(Port, 4566)
                 .WithCleanUp(true)
                 .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(4566))
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
}
