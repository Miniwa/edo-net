using System;
using System.Runtime.InteropServices;

namespace Edo.Win32.Native
{
    /// <summary>
    /// Wraps functions and structures imported from ntdll.dll of the windows api
    /// </summary>
    public static class NtDll
    {
        /// <summary>
        /// Represents the windows api structure SYSTEM_HANDLE
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemHandle
        {
            public UInt32 ProcessId;
            public HandleType Type;
            public Byte Flags;
            public UInt16 Handle;
            public IntPtr Address;
            public ProcessRights Rights;
        }

        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern NtStatus NtQuerySystemInformation([In] SystemInformationType infoType, byte[] buffer,
            [In] UInt32 size, ref UInt32 actualSize);
    }
}