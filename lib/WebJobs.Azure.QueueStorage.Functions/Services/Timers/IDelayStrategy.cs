namespace WebJobs.Azure.QueueStorage.Functions.Services.Timers
{
    public interface IDelayStrategy
    {
        TimeSpan MinimalRenewalDelay { get; set; }
        TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null);
    }
}
