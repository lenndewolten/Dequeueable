namespace WebJobs.Azure.QueueStorage.Functions.Services.Singleton
{
    public class SingletonException : Exception
    {
        public SingletonException(string message) : base(message)
        {
        }

        public SingletonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
