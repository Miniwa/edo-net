using System;
using System.Runtime.InteropServices;

namespace Edo.Windows
{
    /// <summary>
    /// Represents the windows api structure MODULEENTRY32
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ModuleEntry32
    {
        public UInt32 StructSize;
        public UInt32 ModuleId;
        public UInt32 ProcessId;
        public UInt32 LoadCount1;
        public UInt32 LoadCount2;
        public IntPtr BaseAddress;
        public UInt32 BaseSize;
        public IntPtr Handle;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MaxModuleName32 + 1)]
        public String FileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MaxPath)]
        public String FullPath;
    }
}