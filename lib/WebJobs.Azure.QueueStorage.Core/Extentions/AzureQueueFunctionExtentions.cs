using System.Reflection;
using WebJobs.Azure.QueueStorage.Core.Attributes;

namespace WebJobs.Azure.QueueStorage.Core.Extentions
{
    public static class AzureQueueFunctionExtentions
    {
        public static SingletonAttribute? GetSingletonAttribute(this IAzureQueueFunction function)
        {
            return function.GetType().GetCustomAttribute<SingletonAttribute>();
        }

        public static SingletonAttribute? GetSingletonAttribute(this Type type)
        {
            return typeof(IAzureQueueFunction).IsAssignableFrom(type) ? type.GetCustomAttribute<SingletonAttribute>() : null;
        }
    }
}
