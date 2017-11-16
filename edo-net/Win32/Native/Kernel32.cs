using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Edo.Win32.Native
{
    /// <summary>
    /// Wraps functions and structures imported from kernel32.dll of the windows api
    /// </summary>
    public static class Kernel32
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

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenProcess([In] ProcessRights desiredRights, [In] Boolean inheritHandle,
            [In] UInt32 processId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle([In] String name);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean DuplicateHandle([In] IntPtr sourceProcessHandle, [In] IntPtr sourceHandle,
            [In] IntPtr targetProcessHandle, ref IntPtr targetHandle, [In] UInt32 desiredRights,
            [In] Boolean inherit, [In] DuplicationOptions duplicationOptions);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean CloseHandle([In] IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress([In] IntPtr module, [In] String procName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean ReadProcessMemory([In] IntPtr processHandle, [In] IntPtr address,
            [Out] byte[] buffer,
            [In] UIntPtr nrBytesToRead, [Out] out UIntPtr nrBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean WriteProcessMemory([In] IntPtr processHandle, [In] IntPtr address,
            [Out] byte[] buffer, [In] UIntPtr nrBytesToWrite, [Out] out UIntPtr nrBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr VirtualAllocEx([In] IntPtr processHandle, [In] IntPtr address, [In] UIntPtr size,
            [In] AllocationOptions allocationType, [In] ProtectionOptions protectionOptions);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean VirtualFreeEx([In] IntPtr processHandle, [In] IntPtr address, [In] UIntPtr size,
            [In] FreeOptions freeType);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern UInt32 GetProcessId([In] IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean QueryFullProcessImageName([In] IntPtr processHandle, [In] UInt32 flags,
            [Out] StringBuilder path, ref UInt32 size);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateToolhelp32Snapshot([In] SnapshotFlags flags, [In] UInt32 processId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean Module32First([In] IntPtr snapshot, ref ModuleEntry32 moduleEntry);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean Module32Next([In] IntPtr snapshot, ref ModuleEntry32 moduleEntry);
    }
}