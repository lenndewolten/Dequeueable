using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Functions;
using WebJobs.Azure.QueueStorage.Functions.Models;

namespace WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Functions
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
                _logger.LogInformation("Executing job loop {I}", i);
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Job cancelled!");
                    break;
                }
                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
