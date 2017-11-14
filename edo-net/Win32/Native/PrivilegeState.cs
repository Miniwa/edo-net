using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of available privilege states in the windows api
    /// </summary>
    public enum PrivilegeState : UInt32
    {
        /// <summary>
        /// Represents SE_PRIVILEGE_ENABLED_BY_DEFAULT
        /// </summary>
        EnabledByDefault = 0x00000001,

        /// <summary>
        /// Represents SE_PRIVILEGE_ENABLED
        /// </summary>
        Enabled = 0x00000002,

        /// <summary>
        /// Represents SE_PRIVILEGE_REMOVED
        /// </summary>
        Removed = 0x00000004,

        /// <summary>
        /// Represents SE_PRIVILEGE_USED_FOR_ACCESS
        /// </summary>
        UsedForAccess = 0x80000000
    }
}