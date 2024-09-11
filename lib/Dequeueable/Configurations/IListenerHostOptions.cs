namespace Dequeueable.Configurations
{
    /// <summary>
    /// Host options to configure the settings of the host and it's queue listeners
    /// </summary>
    public interface IListenerHostOptions
    {
        /// <summary>
        /// The threshold at which a new batch of messages will be fetched.
        /// </summary>
        int NewBatchThreshold { get; set; }

        /// <summary>
        /// The minimum polling interval to check the queue for new messages.
        /// </summary>
        long MinimumPollingIntervalInMilliseconds { get; set; }

        /// <summary>
        /// The maximum polling interval to check the queue for new messages. 
        /// </summary>
        long MaximumPollingIntervalInMilliseconds { get; set; }

        /// <summary>
        /// The delta used to randomize the polling interval.
        /// </summary>
        TimeSpan? DeltaBackOff { get; set; }
    }
}
