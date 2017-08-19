using System;

namespace Edo.Win32
{
    /// <summary>
    /// An enumeration of available system information types when calling NtQuerySystemInformation
    /// </summary>
    public enum SystemInformationType : UInt32
    {
        /// <summary>
        /// Represents SystemHandleInformation 0x0010
        /// </summary>
        HandleInformation = 0x0010,
    }
}