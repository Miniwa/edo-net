using System;
using System.Runtime.InteropServices;

namespace Edo.Win32.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemHandle
    {
        public UInt32 ProcessId;
        public Byte ObjectType;
        public Byte Flags;
        public UInt16 Handle;
        public IntPtr Address;
        public ProcessRights Rights;
    }
}