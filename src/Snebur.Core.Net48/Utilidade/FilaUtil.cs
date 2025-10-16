using System.Collections.Concurrent;

namespace Snebur.Utilidade
{
    public static class FilaUtil
    {
        public static void Limpar<T>(ConcurrentQueue<T> fila)
        {
            T item;
            while (fila.TryDequeue(out item) || fila.Count > 0)
            {
            }
        }
    }
}