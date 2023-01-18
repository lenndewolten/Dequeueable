using Dequeueable.AzureQueueStorage.Attributes;
using System.Reflection;

namespace Dequeueable.AzureQueueStorage.Extentions
{
    internal static class AzureQueueFunctionExtentions
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
