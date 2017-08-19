using System;

namespace Edo.Win32
{
    /// <summary>
    /// An enumeration of all available handle duplication options in the windows api
    /// </summary>
    public enum DuplicationOptions : UInt32
    {
        /// <summary>
        /// Represents no options
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents DUPLICATE_CLOSE_SOURCE 0x00000001
        /// </summary>
        CloseSource = 0x1,

        /// <summary>
        /// Represents DUPLICATE_SAME_ACCESS 0x00000002
        /// </summary>
        SameAccess = 0x2,
    }
}