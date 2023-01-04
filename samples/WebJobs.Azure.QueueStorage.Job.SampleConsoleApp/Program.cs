using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Job.Extentions;
using WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Job;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddAzureQueueStorageJob<TestJob>(options =>
    {
        //// Uncomment for identity flow
        //options.AuthenticationScheme = new DefaultAzureCredential();
        //options.AccountName = "storageaccountname";
    });
})
.RunConsoleAsync();