using System;
using System.Runtime.InteropServices;

namespace Edo.Win32.Model
{
    /// <summary>
    /// Represents the windows api structure SYSTEM_HANDLE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemHandle32
    {
        public UInt32 ProcessId;
        public Byte ObjectType;
        public Byte Flags;
        public UInt16 Handle;
        public IntPtr Address;
        public ProcessRights Rights;
    }
}