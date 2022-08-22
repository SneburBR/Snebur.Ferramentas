using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snebur.VisualStudio.DteExtensao
{
    [ComImport]
    [Guid("866311E6-C887-4143-9833-645F5B93F6F1")]
    [TypeLibType(4160)]
    [DefaultMember("Name")]
    public interface Project
    {
        [DispId(0)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(109)]
        string FileName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(109)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }


        [DispId(204)]
        string UniqueName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(204)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }


    }
}
 