using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace Edo.Win32
{
    /// <summary>
    /// Represents a single handle currently active in the system
    /// </summary>
    public struct SystemHandle
    {
        /// <summary>
        /// Initializes the system handle with given process id, handle and process rights
        /// </summary>
        /// <param name="processId">The id of the process that owns the handle</param>
        /// <param name="type">The type of the handle</param>
        /// <param name="handle">The handle represented as an IntPtr</param>
        /// <param name="rights">The rights held by this handle in the context of its target process</param>
        public SystemHandle(Int32 processId, HandleType type, IntPtr handle, ProcessRights rights)
        {
            ProcessId = processId;
            Type = type;
            Handle = handle;
            Rights = rights;
        }

        /// <summary>
        /// Duplicates and returns a clone of this handle that holds equal access rights in the context of its target process
        /// </summary>
        /// <param name="inherit">Whether the handle should be inheritable</param>
        /// <returns>The duplicated handle</returns>
        public SafeProcessHandle Duplicate(Boolean inherit)
        {
            Win32Process process = Win32Process.Open(ProcessId, ProcessRights.DuplicateHandle);
            return process.DuplicateHandle(Handle, ProcessRights.None, inherit, DuplicationOptions.SameAccess);
        }

        /// <summary>
        /// Returns whether or not this handle targets the process with given process id
        /// </summary>
        /// <param name="targetId">The process id to filter by</param>
        /// <returns>A boolean indicating whether this handle targets given process</returns>
        /// <exception cref="ArgumentException">If target id is equal to or less than zero</exception>
        public Boolean TargetsProcess(Int32 targetId)
        {
            if (targetId <= 0)
                throw new ArgumentException("Process id must be larger than zero");

            try
            {
                Win32Process process = Win32Process.Open(ProcessId, ProcessRights.DuplicateHandle);
                SafeProcessHandle targetHandle = process.DuplicateHandle(Handle, ProcessRights.QueryInformation,
                    false, DuplicationOptions.None);

                return Convert.ToInt32(Kernel32.GetProcessId(targetHandle.DangerousGetHandle())) == targetId;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether this handle holds at least given access rights in the context of its target process
        /// </summary>
        /// <param name="rights">The minimum access rights required</param>
        /// <returns>A boolean indicating whether this handle holds at minimum given access rights</returns>
        public Boolean HasRights(ProcessRights rights)
        {
            return (rights & Rights) == rights;
        }

        /// <summary>
        /// The id of the process that owns this handle
        /// </summary>
        public Int32 ProcessId { get; private set; }

        /// <summary>
        /// The type of this handle
        /// </summary>
        public HandleType Type { get; private set; }

        /// <summary>
        /// A handle to a process represented as an IntPtr
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// The access rights held by this handle in the context of its target process
        /// </summary>
        public ProcessRights Rights { get; private set; }
    }
}