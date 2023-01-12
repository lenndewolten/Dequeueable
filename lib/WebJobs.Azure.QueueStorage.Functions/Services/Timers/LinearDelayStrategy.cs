namespace WebJobs.Azure.QueueStorage.Functions.Services.Timers
{
    public sealed class LinearDelayStrategy : IDelayStrategy
    {
        public TimeSpan MinimalRenewalDelay { get; set; }
        internal int Divisor { get; set; } = 2;

        public LinearDelayStrategy(TimeSpan minimalRenewalDelay)
        {
            MinimalRenewalDelay = minimalRenewalDelay;
        }

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
