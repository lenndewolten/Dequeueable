using WebJobs.Azure.QueueStorage.Core.Configurations;

namespace WebJobs.Azure.QueueStorage.Function.Configurations
{
    public class FunctionOptions : HostOptions
    {
        private TimeSpan _minimumPollingInterval = TimeSpan.FromMilliseconds(5);
        private TimeSpan _maximumPollingInterval = TimeSpan.FromSeconds(3);
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

        public TimeSpan MinimumPollingInterval
        {
            get => _minimumPollingInterval;
            set
            {
                if (value.Ticks < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumPollingInterval), $"'{nameof(MinimumPollingInterval)}' must not be negative.");
                }

                if (value.Ticks > MaximumPollingInterval.Ticks)
                {
                    throw new ArgumentException($"The '{nameof(MinimumPollingInterval)}' must not be greater than the '{nameof(MaximumPollingInterval)}'.",
                        nameof(MinimumPollingInterval));
                }

                _minimumPollingInterval = value;
            }
        }
        public TimeSpan MaximumPollingInterval
        {
            get => _maximumPollingInterval;
            set
            {
                if (value.Ticks < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumPollingInterval), $"'{nameof(MaximumPollingInterval)}' must not be negative or zero.");
                }

                if (value.Ticks < MinimumPollingInterval.Ticks)
                {
                    throw new ArgumentException($"The '{nameof(MaximumPollingInterval)}' must not be smaller than the '{nameof(MinimumPollingInterval)}'.",
                        nameof(MaximumPollingInterval));
                }

                _maximumPollingInterval = value;
            }
        }

        public TimeSpan? DeltaBackOff { get; set; }
    }
}
