using System;

namespace Edo.Win32
{
    /// <summary>
    /// Contains constants used by the windows api
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Represents INVALID_HANDLE_VALUE
        /// </summary>
        public const Int32 InvalidHandleValue = -1;

        /// <summary>
        /// Represents MAX_PATH
        /// </summary>
        public const Int32 MaxPath = 260;

        /// <summary>
        /// Represents MAX_MODULE_NAME32
        /// </summary>
        public const Int32 MaxModuleName32 = 255;
    }
}