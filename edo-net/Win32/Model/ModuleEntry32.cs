using System;
using System.Runtime.InteropServices;

namespace Edo.Win32.Model
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

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constant.MaxModuleName32 + 1)]
        public String FileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constant.MaxPath)]
        public String FullPath;
    }
}