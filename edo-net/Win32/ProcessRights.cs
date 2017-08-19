using System;

namespace Edo.Win32
{
    /// <summary>
    /// An enumeration of available process flags available in the windows api
    /// </summary>
    [Flags]
    public enum ProcessRights : UInt32
    {
        /// <summary>
        /// Represents no access rights what so ever
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents PROCESS_CREATE_PROCESS (0x0080)
        /// </summary>
        CreateProcess = 0x80,

        /// <summary>
        /// Represents PROCESS_CREATE_THREAD (0x0002)
        /// </summary>
        CreateThread = 0x2,

        /// <summary>
        /// Represents PROCESS_DUP_HANDLE (0x0040)
        /// </summary>
        DuplicateHandle = 0x40,

        /// <summary>
        /// Represents PROCESS_QUERY_INFORMATION (0x0400)
        /// </summary>
        QueryInformation = 0x400,

        /// <summary>
        /// Represents PROCESS_QUERY_LIMITED_INFORMATION (0x1000)
        /// </summary>
        QueryLimitedInformation = 0x1000,

        /// <summary>
        /// Represents PROCESS_SET_INFORMATION (0x0200)
        /// </summary>
        SetInformation = 0x200,

        /// <summary>
        /// Represents PROCESS_SET_QUOTA (0x0100)
        /// </summary>
        SetQuota = 0x100,

        /// <summary>
        /// Represents PROCESS_SUSPEND_RESUME (0x0800)
        /// </summary>
        SuspendResume = 0x800,

        /// <summary>
        /// Represents PROCESS_TERMINATE (0x0001)
        /// </summary>
        Terminate = 0x1,

        /// <summary>
        /// Represents PROCESS_VM_OPERATION (0x0008)
        /// </summary>
        VmOperation = 0x8,

        /// <summary>
        /// Represents PROCESS_VM_READ (0x0010)
        /// </summary>
        VmRead = 0x10,

        /// <summary>
        /// Represents PROCESS_VM_WRITE (0x0020)
        /// </summary>
        VmWrite = 0x20,

        /// <summary>
        /// Represents SYNCHRONIZE (0x00100000L)
        /// </summary>
        Synchronize = 0x100000,

        /// <summary>
        /// Represents PROCESS_ALL_ACCESS
        /// </summary>
        AllAccess = CreateProcess | CreateThread | DuplicateHandle | QueryInformation | QueryLimitedInformation
            | SetInformation | SetQuota | SuspendResume | Terminate | VmOperation | VmRead | VmWrite | Synchronize
            | StandardRights.Synchronize | StandardRights.Delete | StandardRights.ReadControl
            | StandardRights.WriteDac | StandardRights.WriteOwner
    }
}