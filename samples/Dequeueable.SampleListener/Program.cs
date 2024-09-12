using Dequeueable.Configurations;
using Dequeueable.SampleListener;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddDequeueableServices<SampleMessage>()
        .WithQueueMessageManager<SampleQueueMessageManager>()
        .WithQueueMessageHandler<SampleQueueMessageHandler>()
        .AsListener(opt =>
        {
            opt.NewBatchThreshold = 2;
        });
})
.RunConsoleAsync();