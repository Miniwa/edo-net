using System;
using Microsoft.Win32.SafeHandles;

namespace Edo.Win32
{
    /// <summary>
    /// Represents a link between a process and a handle owned by that process
    /// </summary>
    public struct HandlePair
    {
        public HandlePair(Win32Process process, IntPtr handle)
        {
            Process = process;
            Handle = handle;
        }

        /// <summary>
        /// The process that owns the handle
        /// </summary>
        public Win32Process Process { get; private set; }

        /// <summary>
        /// The handle owned by the process
        /// </summary>
        public IntPtr Handle { get; private set; }
    }
}