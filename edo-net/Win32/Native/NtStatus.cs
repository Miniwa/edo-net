using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// An enumeration of all NTSTATUS vales in the windows api
    /// </summary>
    public enum NtStatus : UInt32
    {
        /// <summary>
        /// Represents STATUS_SUCCESS
        /// </summary>
        Success = 0x00000000,

        /// <summary>
        /// Represents STATUS_INFO_LENGTH_MISMATCH
        /// </summary>
        InfoLengthMismatch = 0xC0000004,

        /// <summary>
        /// Represents STATUS_BUFFER_TOO_SMALL
        /// </summary>
        BufferTooSmall = 0xC0000023,
    }
}