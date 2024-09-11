using Dequeueable.Configurations;

namespace Dequeueable.UnitTests.Configurations
{
    public sealed class TestListenerHostOptions : IListenerHostOptions
    {
        public int NewBatchThreshold { get; set; }
        public long MinimumPollingIntervalInMilliseconds { get; set; }
        public long MaximumPollingIntervalInMilliseconds { get; set; }
        public TimeSpan? DeltaBackOff { get; set; }
    }
}
