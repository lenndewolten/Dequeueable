using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Hosts
{
    internal sealed class ListenerHost : BackgroundService
    {
        private readonly IHostHandler _hostHandler;
        private readonly ILogger<ListenerHost> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ListenerHost(IHostHandler hostHandler, ILogger<ListenerHost> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostHandler = hostHandler;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _hostHandler.HandleAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred, shutting down the host");
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }
    }
}
