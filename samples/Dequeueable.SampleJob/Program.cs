using Dequeueable.Configurations;
using Dequeueable.SampleJob;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddDequeueableServices<SampleMessage>()
        .WithQueueMessageManager<SampleQueueMessageManager>()
        .WithQueueMessageHandler<SampleQueueMessageHandler>()
        .AsJob();
})
.RunConsoleAsync();