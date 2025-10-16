using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snebur.Utilidade
{
    internal class OperationWithTimeout
    {
        public static Task<T> ExecuteWithTimeout<T>(Func<CancellationToken, T>[] operations,
                                                    double timeout)
        {
            var cancellationToken = new CancellationTokenSource();
            var delayTask = Delay(timeout);
            var tasks = new Task<T>[operations.Length];

            for (var i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                var operationTask = Task.Factory.StartNew(() => operation(cancellationToken.Token),
                                                                          cancellationToken.Token);
                tasks[i] = operationTask;
            }

            var allTakes = new Task[tasks.Length + 1];
            tasks.CopyTo(allTakes, 0);
            allTakes[tasks.Length] = delayTask;
            Task.WaitAny(allTakes);

            try
            {
                if (!tasks.Any(x => x.IsCompleted))
                {
                    cancellationToken.Cancel();
                    throw new TimeoutException($" Tempo limite de {timeout} milissegundos excedido. O método está muito lento :-)");
                }
            }
            finally
            {
                cancellationToken.Dispose();
            }

            return tasks.FirstOrDefault(x => x.IsCompleted) ??
                   tasks.First();
        }

        public static Task<T> ExecuteWithTimeout<T>(Func<CancellationToken, T> operation,
                                                    double timeout)
        {
            var cancellationToken = new CancellationTokenSource();
            var operationTask = Task.Factory.StartNew(() => operation(cancellationToken.Token),
                                                            cancellationToken.Token);

            var delayTask = Delay(timeout);
            var tasks = new Task[] { operationTask, delayTask };

            Task.WaitAny(tasks);

            try
            {
                if (!operationTask.IsCompleted)
                {
                    cancellationToken.Cancel();
                    throw new TimeoutException($" Tempo limite de {timeout} milissegundos excedido. O método está muito lento :-)");
                }
            }
            finally
            {
                cancellationToken.Dispose();
            }
            return operationTask;
        }

        public static Task Delay(double delayTime)
        {
            var completionSource = new TaskCompletionSource<bool>();
            var timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) => completionSource.TrySetResult(true);
            timer.Interval = delayTime;
            timer.AutoReset = false;
            timer.Start();
            return completionSource.Task;
        }
    }
}