using JobHandlers.AzureQueueMessage.Extentions;
using Microsoft.Extensions.Hosting;
using QueueMessages.SampleApp;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAzureQueueMessageService<QueueMessageExecutor>(options => options.VisibilityTimeout = TimeSpan.FromSeconds(60));
    })
    .RunConsoleAsync();