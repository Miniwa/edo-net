using System;

namespace Edo.Win32
{
    /// <summary>
    /// Represents information about a thread of a process
    /// </summary>
    public class ThreadInfo
    {
        public ThreadInfo(Int32 id, Int32 processId, Int32 basePriority)
        {
            Id = id;
            ProcessId = processId;
            BasePriority = basePriority;
        }

        /// <summary>
        /// The unique identifier of the thread
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// The unique identifier of the process that owns this thread
        /// </summary>
        public Int32 ProcessId { get; set; }

        /// <summary>
        /// The kernel base priority assigned to this thread. This is a number from 0 to 31
        /// with 0 representing the lowest possible priority
        /// </summary>
        public Int32 BasePriority { get; set; }
    }
}