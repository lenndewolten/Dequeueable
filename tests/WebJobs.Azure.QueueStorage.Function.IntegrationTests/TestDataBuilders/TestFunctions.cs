﻿using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Function.IntegrationTests.TestDataBuilders
{
    public class TestFunction : IAzureQueueFunction
    {
        private readonly IFakeService _fakeService;

        public TestFunction(IFakeService fakeService)
        {
            _fakeService = fakeService;
        }

        public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            return _fakeService.Execute(message);
        }
    }

    [Singleton(scope: "Id", containerName: ContainerName, minimumIntervalSeconds: 1)]
    public class SingletonFunction : IAzureQueueFunction
    {
        public const string ContainerName = "scopedtestcontainer";
        private readonly IFakeService _fakeService;

        public SingletonFunction(IFakeService fakeService)
        {
            _fakeService = fakeService;
        }

        public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            return _fakeService.Execute(message);
        }
    }

    public interface IFakeService
    {
        Task Execute(Message message);
    }

    public class FakeService : IFakeService
    {

        public Task Execute(Message message) { return Task.CompletedTask; }
    }
}