using Amazon.Runtime;
using Amazon.SQS;
using Dequeueable.AmazonSQS.Extentions;
using Dequeueable.AmazonSQS.SampleListener.Functions;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services
    .AddAmazonSQSServices<TestFunction>()
    .RunAsListener(options =>
    {
        options.AWSCredentials = new BasicAWSCredentials("dummy", "dummy");
        options.AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = "http://localhost:4566" };
        options.VisibilityTimeoutInSeconds = 300;
        options.BatchSize = 4;
    });
})
.RunConsoleAsync();