namespace System.Collections
{
    public static class ConcurrentQueueExtensao
    {
        public static void Clear<T>(this Concurrent.ConcurrentQueue<T> origem)
        {
            while (origem.Count > 0)
            {
                origem.TryDequeue(out T item);
            }
        }
    }
}