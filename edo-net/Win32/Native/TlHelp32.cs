using System;
using System.Runtime.InteropServices;

namespace Edo.Win32.Native
{
    /// <summary>
    /// Wraps structures imported from tlhelp32.h of the windows api
    /// </summary>
    public static class TlHelp32
    {
        public struct ThreadEntry32
        {
            public UInt32 StructSize;
            public UInt32 Usage;
            public UInt32 ThreadId;
            public UInt32 ProcessId;
            public Int32 Priority;
            public Int32 DeltaPriority;
            public UInt32 Flags;
        }
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
}