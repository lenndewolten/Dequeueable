namespace WebJobs.Azure.QueueStorage.Functions.Configurations
{
    public class ListenerOptions : HostOptions
    {
        private long _minimumPollingIntervalInMilliseconds = 5;
        private long _maximumPollingIntervalInMilliseconds = 10000;
        private int? _newBatchThreshold;

        public int NewBatchThreshold
        {
            get => _newBatchThreshold ?? Convert.ToInt32(Math.Ceiling(BatchSize / (double)2));
            set
            {
                if (value > BatchSize)
                {
                    throw new ArgumentException($"'{nameof(NewBatchThreshold)}' cannot be bigger than {nameof(BatchSize)}.", nameof(NewBatchThreshold));
                }

                _newBatchThreshold = value;
            }
        }

        public long MinimumPollingIntervalInMilliseconds
        {
            get => _minimumPollingIntervalInMilliseconds;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumPollingIntervalInMilliseconds), $"'{nameof(MinimumPollingIntervalInMilliseconds)}' must not be negative.");
                }

                _minimumPollingIntervalInMilliseconds = value;
            }
        }
        public long MaximumPollingIntervalInMilliseconds
        {
            get => _maximumPollingIntervalInMilliseconds;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumPollingIntervalInMilliseconds), $"'{nameof(MaximumPollingIntervalInMilliseconds)}' must not be negative or zero.");
                }

                _maximumPollingIntervalInMilliseconds = value;
            }
        }

        public TimeSpan? DeltaBackOff { get; set; }
    }
}
