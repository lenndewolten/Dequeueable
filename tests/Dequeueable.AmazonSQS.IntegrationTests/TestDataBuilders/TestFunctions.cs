using Dequeueable.AmazonSQS.Models;

namespace Dequeueable.AmazonSQS.IntegrationTests.TestDataBuilders
{
    public class TestFunction : IAmazonSQSFunction
    {
        private readonly IFakeService _fakeService;

        public TestFunction(IFakeService fakeService)
        {
            _fakeService = fakeService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            await _fakeService.Execute(message);
        }
    }

    public interface IFakeService
    {
        Task Execute(Message message);
    }

    public class FakeService : IFakeService
    {
        public Task Execute(Message message) { return Task.CompletedTask; }
    }

    public class SingletonFakeService : IFakeService
    {
        private readonly static SemaphoreSlim _lock = new(1, 1);


        public async Task Execute(Message message)
        {

            if (_lock.Wait(TimeSpan.FromMilliseconds(1)))
            {
                await Task.Delay(10);
                _lock.Release();
                return;
            }

            throw new InvalidOperationException();
        }
    }
}
