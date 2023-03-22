using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class WorkThreadUtil
    {
        public static SwitchToWorkThreadAwaitable SwitchToWorkerThreadAsync()
        {
            return new SwitchToWorkThreadAwaitable();
        }
    }
    public struct SwitchToWorkThreadAwaitable : INotifyCompletion
    {
        public SwitchToWorkThreadAwaitable GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }

        public bool IsCompleted
        {
            get
            {
                var checkAccess = AplicacaoSnebur.Atual.VisualStudio().CheckAccess();
                return !checkAccess;
            }
        }

        public void OnCompleted(Action continuation)
        {
            Task.Run(continuation);
            //_dispatcher.BeginInvoke(continuation);
        }
    }
}
