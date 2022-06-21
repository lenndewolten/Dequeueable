using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobHandlers.AzureQueueMessage.IntegrationTests
{
    internal class TestQueueMessageExecutor : IQueueMessageExecutor
    {
        public Task Execute(QueueMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
