﻿using Dequeueable.AzureQueueStorage.Extentions;
using Dequeueable.AzureQueueStorage.SampleListener.Function;
using Microsoft.Extensions.Hosting;

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