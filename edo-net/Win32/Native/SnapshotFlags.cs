using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of all available Toolhelp32 snapshot flags in the windows api
    /// </summary>
    [Flags]
    public enum SnapshotFlags : UInt32
    {
        /// <summary>
        /// Represents TH32CS_INHERIT (0x80000000)
        /// </summary>
        Inherit = 0x80000000,

        /// <summary>
        /// Represents TH32CS_SNAPHEAPLIST (0x00000001)
        /// </summary>
        HeapList = 0x1,

        /// <summary>
        /// Represents TH32CS_SNAPMODULE (0x00000008)
        /// </summary>
        Module = 0x8,

        /// <summary>
        /// Represents TH32CS_SNAPMODULE32 0x00000010
        /// </summary>
        Module32 = 0x10,

        /// <summary>
        /// Represents TH32CS_SNAPPROCESS (0x00000002)
        /// </summary>
        Process = 0x2,

        /// <summary>
        /// Represents TH32CS_SNAPTHREAD (0x00000004)
        /// </summary>
        Thread = 0x4,

        /// <summary>
        /// Represents TH32CS_SNAPNOHEAPS (0x40000000)
        /// Must be used in combination with any other flag to avoid memory errors when calling from managed process
        /// </summary>
        NoHeaps = 0x40000000,

        /// <summary>
        /// Represents TH32CS_SNAPALL
        /// </summary>
        SnapAll = HeapList | Module | Process | Thread
    }
}