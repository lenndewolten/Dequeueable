using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class JobHost : BackgroundService
    {
        private readonly ILogger<JobHost> _logger;
        private readonly IHost _hostHandler;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public JobHost(IHost hostHandler, IHostApplicationLifetime hostApplicationLifetime, ILogger<JobHost> logger)
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
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "Unhandled exception occurred, shutting down the host");
                throw;
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
