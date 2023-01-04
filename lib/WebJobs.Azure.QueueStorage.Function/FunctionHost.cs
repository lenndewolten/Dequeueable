using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Function.Services;

namespace WebJobs.Azure.QueueStorage.Function
{
    internal sealed class FunctionHost : BackgroundService
    {
        private readonly IQueueListener _queueListener;
        private readonly ILogger<FunctionHost> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public FunctionHost(IQueueListener queueListener, ILogger<FunctionHost> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _queueListener = queueListener;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _queueListener.ListenAsync(stoppingToken);
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
