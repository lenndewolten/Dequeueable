using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AmazonSQS.Services.Hosts
{
    internal sealed class JobHost : BackgroundService
    {
        private readonly IHostExecutor _hostExecutor;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<JobHost> _logger;

        public JobHost(IHostExecutor hostExecutor, IHostApplicationLifetime hostApplicationLifetime, ILogger<JobHost> logger)
        {
            _hostExecutor = hostExecutor;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _hostExecutor.HandleAsync(stoppingToken);
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
