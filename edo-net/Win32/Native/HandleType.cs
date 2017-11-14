using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of available handle types in the windows api
    /// </summary>
    public enum HandleType : Byte
    {
        /// <summary>
        /// No type at all
        /// </summary>
        None = 0,

        /// <summary>
        /// A handle to a process
        /// </summary>
        Process = 7,
    }
}