using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AmazonSQS.Services.Hosts
{
    internal sealed class QueueListenerHost : BackgroundService
    {
        private readonly IHostExecutor _hostExecutor;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<QueueListenerHost> _logger;

        public QueueListenerHost(IHostExecutor hostExecutor, IHostApplicationLifetime hostApplicationLifetime, ILogger<QueueListenerHost> logger)
        {
            _hostExecutor = hostExecutor;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _hostExecutor.HandleAsync(stoppingToken);
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
