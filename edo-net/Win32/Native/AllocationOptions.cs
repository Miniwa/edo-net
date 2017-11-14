using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of available allocation types within the windows api
    /// </summary>
    [Flags]
    public enum AllocationOptions : UInt32
    {
        /// <summary>
        /// Represents MEM_COMMIT 0x00001000
        /// </summary>
        Commit = 0x1000,

        /// <summary>
        /// Represents MEM_RESERVE 0x00002000
        /// </summary>
        Reserve = 0x2000,

        /// <summary>
        /// Represents MEM_RESET 0x00080000
        /// </summary>
        Reset = 0x80000,

        /// <summary>
        /// Represents MEM_RESET_UNDO 0x1000000
        /// </summary>
        ResetUndo = 0x1000000,

        /// <summary>
        /// Represents MEM_LARGE_PAGES 0x20000000
        /// </summary>
        LargePages = 0x20000000,

        /// <summary>
        /// Represents MEM_PHYSICAL 0x00400000
        /// </summary>
        Physical = 0x400000,

        /// <summary>
        /// Represents MEM_TOP_DOWN 0x00100000
        /// </summary>
        TopDown = 0x100000
    }
}