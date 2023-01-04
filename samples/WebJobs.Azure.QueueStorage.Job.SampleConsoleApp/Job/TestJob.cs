using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job
{
    //[Singleton("MyAwesomeProperty")]
    internal class TestJob : IAzureQueueJob
    {
        private readonly ILogger<TestJob> _logger;

        public TestJob(ILogger<TestJob> logger)
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
