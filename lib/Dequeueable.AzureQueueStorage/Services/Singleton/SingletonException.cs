namespace Dequeueable.AzureQueueStorage.Services.Singleton
{
    internal class SingletonException : Exception
    {
        public SingletonException(string message) : base(message)
        {
        }

        public SingletonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
