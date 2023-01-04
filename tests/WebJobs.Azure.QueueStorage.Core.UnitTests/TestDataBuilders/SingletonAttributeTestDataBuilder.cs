using WebJobs.Azure.QueueStorage.Core.Attributes;

namespace WebJobs.Azure.QueueStorage.Core.UnitTests.TestDataBuilders
{
    public class SingletonAttributeTestDataBuilder
    {
        private TimeSpan _minimumInterval = TimeSpan.Zero;
        private TimeSpan _maximumInterval = TimeSpan.Zero.Add(TimeSpan.FromMilliseconds(1));
        private string _scope = "id";
        private int _maxRetries;

        public SingletonAttribute Build()
        {
            return new SingletonAttribute(_scope)
            {
                MinimumInterval = _minimumInterval,
                MaximumInterval = _maximumInterval,
                MaxRetries = _maxRetries
            };
        }

        public SingletonAttributeTestDataBuilder WithMinimumInterval(TimeSpan minimumInterval)
        {
            _minimumInterval = minimumInterval;
            return this;
        }
        public SingletonAttributeTestDataBuilder WithMaximumInterval(TimeSpan maximumInterval)
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
