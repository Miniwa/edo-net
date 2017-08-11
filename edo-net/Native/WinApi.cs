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
        public static extern IntPtr OpenProcess(Int32 desiredAccess, Boolean inheritHandle, Int32 processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadProcessMemory(IntPtr processHandle, IntPtr address, byte[] buffer,
            Int32 nrBytesToRead, ref Int32 nrBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean WriteProcessMemory(IntPtr processHandle, IntPtr address, byte[] buffer,
            Int32 nrBytesToWrite, ref Int32 nrBytesWritten);

        // Process right constants
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_ALL_ACCESS = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;
    }
}