using System.ComponentModel.DataAnnotations;

namespace Dequeueable.AzureQueueStorage.Configurations
{
    /// <summary>
    /// Host options to configure the settings of the host and it's queue listeners
    /// </summary>
    public class ListenerOptions : HostOptions
    {
        private int? _newBatchThreshold;

        /// <summary>
        /// The threshold at which a new batch of messages will be fetched.
        /// </summary>
        public int NewBatchThreshold
        {
            get => _newBatchThreshold ?? Convert.ToInt32(Math.Ceiling(BatchSize / (double)2));
            set
            {
                _newBatchThreshold = value;
            }
        }

        /// <summary>
        /// The minimum polling interval to check the queue for new messages.
        /// </summary>
        [Range(1, long.MaxValue, ErrorMessage = "Value for {0} must not be negative.")]
        public long MinimumPollingIntervalInMilliseconds { get; set; } = 5;

        /// <summary>
        /// The maximum polling interval to check the queue for new messages. 
        /// </summary>
        [Range(1, long.MaxValue, ErrorMessage = "Value for {0} must not be negative or zero.")]
        public long MaximumPollingIntervalInMilliseconds { get; set; } = 10000;

        /// <summary>
        /// The delta used to randomize the polling interval.
        /// </summary>
        public TimeSpan? DeltaBackOff { get; set; }

        internal static bool ValidatePollingInterval(ListenerOptions options)
        {
            return options.MinimumPollingIntervalInMilliseconds < options.MaximumPollingIntervalInMilliseconds;
        }

        internal static bool ValidateNewBatchThreshold(ListenerOptions options)
        {
            return options._newBatchThreshold is null || options.NewBatchThreshold <= options.BatchSize;
        }
    }
}
