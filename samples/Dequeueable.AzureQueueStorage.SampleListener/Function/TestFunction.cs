using Dequeueable.AzureQueueStorage.Models;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.SampleListener.Function
{
    //[Singleton("MyAwesomeProperty")]
    internal class TestFunction : IAzureQueueFunction
    {
        private readonly ILogger<TestFunction> _logger;

        public TestFunction(ILogger<TestFunction> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            for (var i = 0; i < 6; i++)
            {
                _logger.LogInformation("Executing function loop {I}", i);
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Function cancelled!");
                    break;
                }
                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
