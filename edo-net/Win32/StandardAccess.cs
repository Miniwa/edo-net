using System;

namespace Edo.Win32
{
    /// <summary>
    /// An enumeration of standard access flags within the windows api
    /// </summary>
    [Flags]
    public enum StandardAccess : UInt32
    {
        /// <summary>
        /// Represents DELETE (0x00010000L)
        /// </summary>
        Delete = 0x10000,

        /// <summary>
        /// Represents READ_CONTROL (0x00020000L)
        /// </summary>
        ReadControl = 0x20000,

        /// <summary>
        /// Represents SYNCHRONIZE (0x00100000L)
        /// </summary>
        Synchronize = 0x100000,

        /// <summary>
        /// Represents WRITE_DAC (0x00040000L)
        /// </summary>
        WriteDac = 0x40000,

        /// <summary>
        /// Represents WRITE_OWNER (0x00080000L)
        /// </summary>
        WriteOwner = 0x80000
    }
}