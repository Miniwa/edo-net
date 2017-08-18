using System;

namespace Edo.Win32
{
    /// <summary>
    /// An enumeration of available memory protection options in the windows api
    /// </summary>
    [Flags]
    public enum ProtectionOptions : UInt32
    {
        /// <summary>
        /// Represents PAGE_EXECUTE 0x10
        /// </summary>
        Execute = 0x10,

        /// <summary>
        /// Represents PAGE_EXECUTE_READ 0x20
        /// </summary>
        ExecuteRead = 0x20,

        /// <summary>
        /// Represents PAGE_EXECUTE_READWRITE 0x40
        /// </summary>
        ExecuteReadWrite = 0x40,

        /// <summary>
        /// Represents PAGE_EXECUTE_WRITECOPY 0x80
        /// </summary>
        ExecuteWriteCopy = 0x80,

        /// <summary>
        /// Represents PAGE_NOACCESS 0x01
        /// </summary>
        NoAccess = 0x01,

        /// <summary>
        /// Represents PAGE_READONLY 0x02
        /// </summary>
        ReadOnly = 0x02,

        /// <summary>
        /// Represents PAGE_READWRITE 0x04
        /// </summary>
        ReadWrite = 0x04,

        /// <summary>
        /// Represents PAGE_WRITECOPY 0x08
        /// </summary>
        WriteCopy = 0x08,

        /// <summary>
        /// Represents PAGE_TARGETS_INVALID 0x40000000
        /// </summary>
        TargetsInvalid = 0x40000000,

        /// <summary>
        /// Represents PAGE_TARGETS_NO_UPDATE 0x40000000
        /// </summary>
        TargetsNoUpdate = 0x40000000,

        /// <summary>
        /// Represents PAGE_GUARD 0x100
        /// </summary>
        Guard = 0x100,

        /// <summary>
        /// PAGE_NOCACHE 0x200
        /// </summary>
        NoCache = 0x200,

        /// <summary>
        /// PAGE_WRITECOMBINE 0x400
        /// </summary>
        WriteCombine = 0x400
    }
}