using System.ComponentModel.DataAnnotations;

namespace Dequeueable.Configurations
{
    /// <summary>
    /// Host options to configure the settings of the host and it's queue listeners
    /// </summary>
    public class ListenerHostOptions : IListenerHostOptions
    {
        /// <summary>
        /// The threshold at which a new batch of messages will be fetched.
        /// </summary>
        public int NewBatchThreshold { get; set; } = 1;
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
    }
}
