using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Functions.Extentions;
using WebJobs.Azure.QueueStorage.Job.SampleConsoleApp.Functions;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddAzureQueueStorageJob<TestFunction>(options =>
    {
        //// Uncomment for identity flow
        //options.AuthenticationScheme = new DefaultAzureCredential();
        //options.AccountName = "storageaccountname";
    });
})
.RunConsoleAsync();