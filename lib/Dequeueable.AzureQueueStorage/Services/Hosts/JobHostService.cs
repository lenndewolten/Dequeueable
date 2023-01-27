using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class JobHostService : BackgroundService
    {
        private readonly ILogger<JobHostService> _logger;
        private readonly IHost _hostHandler;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public JobHostService(IHost hostHandler, IHostApplicationLifetime hostApplicationLifetime, ILogger<JobHostService> logger)
        {
            _hostHandler = hostHandler;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _hostHandler.HandleAsync(stoppingToken);
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
