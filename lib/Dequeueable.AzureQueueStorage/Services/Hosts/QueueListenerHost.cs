using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class QueueListenerHost(IHostExecutor hostHandler, ILogger<QueueListenerHost> logger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await hostHandler.HandleAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    hostApplicationLifetime.StopApplication();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception occurred, shutting down the host");
                    hostApplicationLifetime.StopApplication();
                }
            }
        }
    }
}
