using System;
using System.Runtime.InteropServices;

namespace Edo.Native
{
    /// <summary>
    /// Wraps methods imported from the windows api
    /// </summary>
    public static class WinApi
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessRights desiredAccess, Boolean inheritHandle, Int32 processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadProcessMemory(IntPtr processHandle, IntPtr address, byte[] buffer,
            Int32 nrBytesToRead, ref Int32 nrBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean WriteProcessMemory(IntPtr processHandle, IntPtr address, byte[] buffer,
            Int32 nrBytesToWrite, ref Int32 nrBytesWritten);
    }
}