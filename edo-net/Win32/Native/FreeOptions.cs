namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of options available when freeing memory in the win api
    /// </summary>
    public enum FreeOptions
    {
        /// <summary>
        /// Represents MEM_DECOMMIT 0x4000
        /// </summary>
        Decommit = 0x4000,

        /// <summary>
        /// MEM_RELEASE 0x8000
        /// </summary>
        Release = 0x8000,
    }
}