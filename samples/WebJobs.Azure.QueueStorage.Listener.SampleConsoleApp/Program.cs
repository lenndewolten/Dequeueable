using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Function.SampleConsoleApp.Function;
using WebJobs.Azure.QueueStorage.Functions.Extentions;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddAzureQueueStorageListener<TestFunction>(options =>
    {
        //// Uncomment for identity flow
        //options.AuthenticationScheme = new DefaultAzureCredential();
        //options.AccountName = "storageaccountname";
    });
})
.RunConsoleAsync();