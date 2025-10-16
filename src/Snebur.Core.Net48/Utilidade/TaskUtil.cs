using System;
using System.Threading.Tasks;
namespace Snebur.Utilidade
{
#if NET40 == false
    public static class TaskUtil
    {
        public static Task Run(Action action)
        {
            return Task.Run(action);
        }

        public static Task<T> Run<T>(Func<T> action)
        {
            return Task.Run(action);
        }

        internal static Task Delay(int delay)
        {
            return Task.Delay(delay);
        }
    }
#endif
}
