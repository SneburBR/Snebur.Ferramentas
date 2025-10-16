using System.Diagnostics;

namespace Snebur.Utilidade
{
    public static class DebugUtil
    {
        private static bool _isAttached = true;
        public static bool IsAttached
            => Debugger.IsAttached && _isAttached;
       
    }
}
