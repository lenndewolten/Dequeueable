namespace WebJobs.Azure.QueueStorage.Functions.Configurations
{
    /// <summary>
    /// Host options to configure the settings of the host and it's queue listeners
    /// </summary>
    public class ListenerOptions : HostOptions
    {
        private long _minimumPollingIntervalInMilliseconds = 5;
        private long _maximumPollingIntervalInMilliseconds = 10000;
        private int? _newBatchThreshold;

        /// <summary>
        /// The threshold at which a new batch of messages will be fetched.
        /// </summary>
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

        /// <summary>
        /// The minimum polling interval to check the queue for new messages.
        /// </summary>
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

        /// <summary>
        /// The maximum polling interval to check the queue for new messages. 
        /// </summary>
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

        /// <summary>
        /// The delta used to randomize the polling interval.
        /// </summary>
        public TimeSpan? DeltaBackOff { get; set; }
    }
}
