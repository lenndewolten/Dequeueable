namespace Dequeueable.Timers
{
    /// <summary>
    /// Defines a strategy for calculating delays for queue message processing.
    /// </summary>
    public interface IDelayStrategy
    {
        /// <summary>
        /// Gets or sets the minimal delay that should be used when no other delay can be computed.
        /// </summary>
        TimeSpan MinimalRenewalDelay { get; set; }

        /// <summary>
        /// Calculates the next delay based on the provided parameters.
        /// </summary>
        /// <param name="nextVisibleOn">
        /// The time when the message should next become visible. If null, the delay computation will be based on other parameters.
        /// </param>
        /// <param name="executionSucceeded">
        /// A flag indicating whether the previous execution succeeded. If false, the minimal renewal delay will be returned.
        /// </param>
        /// <returns>
        /// A <see cref="TimeSpan"/> representing the computed delay. This delay determines how long the system should wait before performing the next action.
        /// </returns>
        TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null);
    }
}
