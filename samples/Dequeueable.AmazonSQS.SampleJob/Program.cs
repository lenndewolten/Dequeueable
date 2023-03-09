using Amazon.SQS;
using Dequeueable.AmazonSQS.Extentions;
using Dequeueable.AmazonSQS.SampleJob.Functions;
using Microsoft.Extensions.Hosting;

var client = new AmazonSQSClient();

//var id = Guid.NewGuid().ToString();
//await client.SendMessageAsync(new SendMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", "MessageGroupId6")
//{
//    MessageGroupId = "6",
//    MessageDeduplicationId = Guid.NewGuid().ToString()
//});

//await client.SendMessageAsync(new SendMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", "MessageGroupId5")
//{
//    MessageGroupId = "5",
//    MessageDeduplicationId = Guid.NewGuid().ToString()
//});

//await client.SendMessageAsync(new SendMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", "MessageGroupId5")
//{
//    MessageGroupId = "5",
//    MessageDeduplicationId = Guid.NewGuid().ToString()
//});

//await client.SendMessageAsync(new SendMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", "MessageGroupId6")
//{
//    MessageGroupId = "6",
//    MessageDeduplicationId = Guid.NewGuid().ToString()
//});

//await client.SendMessageAsync(new SendMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/test-queue", "body 1"));
//var m = await client.ReceiveMessageAsync(new ReceiveMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo") { VisibilityTimeout = 30, MaxNumberOfMessages = 2 });

//await client.DeleteMessageAsync(new DeleteMessageRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", m.Messages[1].ReceiptHandle));

//try
//{

//    var t = await client.ChangeMessageVisibilityBatchAsync(new ChangeMessageVisibilityBatchRequest("https://sqs.eu-central-1.amazonaws.com/808267255128/testqueue.fifo", m.Messages.Select(ma => new ChangeMessageVisibilityBatchRequestEntry { Id = ma.MessageId, ReceiptHandle = ma.ReceiptHandle, VisibilityTimeout = 20 }).ToList()));
//}
//catch (Exception ex)
//{

//}

await Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services
    .AddAmazonSQSServices<TestFunction>()
    .RunAsJob(options =>
    {
        options.VisibilityTimeoutInSeconds = 300;
        options.BatchSize = 4;
    })
    .AsSingleton();
})
.RunConsoleAsync();

