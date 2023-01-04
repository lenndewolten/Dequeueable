using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Function.Extentions;
using WebJobs.Azure.QueueStorage.Function.SampleConsoleApp.Function;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddAzureQueueStorageFunction<TestFunction>(options =>
    {
        //// Uncomment for identity flow
        //options.AuthenticationScheme = new DefaultAzureCredential();
        //options.AccountName = "storageaccountname";
    });
})
.RunConsoleAsync();