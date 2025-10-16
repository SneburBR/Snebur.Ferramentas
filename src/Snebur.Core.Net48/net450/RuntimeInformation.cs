#if NET45 || NET40
 
namespace System.Runtime.InteropServices
{
    public class RuntimeInformation
    {
        internal static bool IsOSPlatform(OSPlatform plarform)
        {
            return plarform.Equals(OSPlatform.Windows);
        }
    }

    public struct OSPlatform : IEquatable<OSPlatform>
    {
        private readonly string _osPlatform;

        public OSPlatform(string osPlatform)
        {
            this._osPlatform = osPlatform;
        }

        public bool Equals(OSPlatform other)
        {
            return this._osPlatform == other._osPlatform;
        }

        public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");
    }
}
 
#endif