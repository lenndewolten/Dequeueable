using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Hosts
{
    internal sealed class JobHost : BackgroundService
    {
        private readonly ILogger<JobHost> _logger;
        private readonly IHostHandler _queueMessagesHandler;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public JobHost(IHostHandler queueMessagesHandler, IHostApplicationLifetime hostApplicationLifetime, ILogger<JobHost> logger)
        {
            _queueMessagesHandler = queueMessagesHandler;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _queueMessagesHandler.HandleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred, shutting down the host");
            }
            finally
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }
    }
}
