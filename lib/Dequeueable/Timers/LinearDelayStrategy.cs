namespace Dequeueable.Timers
{
    /// <summary>
    /// Implements a delay strategy that calculates the next delay based on a linear approach.
    /// </summary>
    public sealed class LinearDelayStrategy(TimeSpan minimalRenewalDelay) : IDelayStrategy
    {
        /// <summary>
        /// Gets or sets the minimal delay that is used if no other delay can be computed.
        /// </summary>
        public TimeSpan MinimalRenewalDelay { get; set; } = minimalRenewalDelay;

        /// <summary>
        /// Gets or sets the divisor used to calculate the delay from the time difference.
        /// </summary>
        public int Divisor { get; set; } = 2;

        /// <summary>
        /// Calculates the next delay based on the time difference between the current time and the specified <paramref name="nextVisibleOn"/> time.
        /// </summary>
        /// <param name="nextVisibleOn">
        /// The time at which the message should next become visible. If null, the delay will default to <see cref="MinimalRenewalDelay"/>.
        /// </param>
        /// <param name="executionSucceeded">
        /// A flag indicating whether the previous execution succeeded. If false, the delay will be <see cref="MinimalRenewalDelay"/>.
        /// </param>
        /// <returns>
        /// The calculated delay as a <see cref="TimeSpan"/>. If the computed delay is negative, <see cref="MinimalRenewalDelay"/> is returned.
        /// </returns>
        public TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null)
        {
            if (executionSucceeded == false)
            {
                return MinimalRenewalDelay;
            }

            var wait = ((nextVisibleOn?.UtcDateTime ?? DateTimeOffset.UtcNow) - DateTimeOffset.UtcNow) / Divisor;
            return wait.Ticks > 0 ? wait : MinimalRenewalDelay;
        }
    }
}
