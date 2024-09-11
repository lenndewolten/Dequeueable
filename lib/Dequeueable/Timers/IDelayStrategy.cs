namespace Dequeueable.Timers
{
    public interface IDelayStrategy
    {
        TimeSpan MinimalRenewalDelay { get; set; }
        TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null);
    }
}
