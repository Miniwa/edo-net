using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Edo.Win32.Native
{
    /// <summary>
    /// Wraps functions and structures imported from Advapi32.dll of the windows api
    /// </summary>
    public static class AdvApi32
    {
        /// <summary>
        /// Represents the structure LUID from the windows api
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Luid
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        /// <summary>
        /// Represents the structure TOKEN_PRIVILEGES from the windows api
        /// </summary>
        public struct TokenPrivileges
        {
            public UInt32 PrivilegeCount;
            public Luid Luid;
            public PrivilegeState State;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean OpenProcessToken([In] IntPtr processHandle, [In] TokenAccessLevels accessLevels,
            ref IntPtr destinationHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean LookupPrivilegeValue([In] String systemName, [In] String name, ref Luid luid);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean AdjustTokenPrivileges([In] IntPtr tokenHandle, [In] Boolean disableAllPrivileges,
            ref TokenPrivileges newState, [In] UInt32 length, IntPtr previousState,
            IntPtr requiredLength);
    }
}