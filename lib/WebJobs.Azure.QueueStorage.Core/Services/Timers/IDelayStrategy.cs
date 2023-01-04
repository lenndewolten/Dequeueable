namespace WebJobs.Azure.QueueStorage.Core.Services.Timers
{
    public interface IDelayStrategy
    {
        TimeSpan MinimalRenewalDelay { get; set; }
        TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null);
    }
}
