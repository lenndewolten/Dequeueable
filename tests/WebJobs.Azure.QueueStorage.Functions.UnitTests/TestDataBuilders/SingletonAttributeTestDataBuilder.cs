using WebJobs.Azure.QueueStorage.Functions.Attributes;

namespace WebJobs.Azure.QueueStorage.Functions.UnitTests.TestDataBuilders
{
    public class SingletonAttributeTestDataBuilder
    {
        private int _minimumInterval = 0;
        private int _maximumInterval = 1;
        private string _scope = "id";
        private int _maxRetries;

        public SingletonAttribute Build()
        {
            return new SingletonAttribute(_scope, maximumIntervalInSeconds: _maximumInterval, minimumIntervalInSeconds: _minimumInterval, maxRetries: _maxRetries);
        }

        public SingletonAttributeTestDataBuilder WithMinimumInterval(int minimumInterval)
        {
            _minimumInterval = minimumInterval;
            return this;
        }
        public SingletonAttributeTestDataBuilder WithMaximumInterval(int maximumInterval)
        {
            _maximumInterval = maximumInterval;
            return this;
        }

        public SingletonAttributeTestDataBuilder WithScope(string scope)
        {
            _scope = scope;
            return this;
        }

        public SingletonAttributeTestDataBuilder WithMaxRetries(int maxRetries)
        {
            _maxRetries = maxRetries;
            return this;
        }
    }
}
