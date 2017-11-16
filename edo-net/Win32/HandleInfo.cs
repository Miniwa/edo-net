using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using Edo.Win32.Native;

namespace Edo.Win32
{
    /// <summary>
    /// Represents information about a handle that is currently active in the system
    /// </summary>
    public struct HandleInfo
    {
        /// <summary>
        /// Initializes the handle with given process id, handle and process rights
        /// </summary>
        /// <param name="processId">The id of the process that owns the handle</param>
        /// <param name="type">The type of the handle</param>
        /// <param name="handle">The handle represented as an IntPtr</param>
        /// <param name="rights">The rights held by this handle in the context of its target process</param>
        public HandleInfo(Int32 processId, HandleType type, IntPtr handle, UInt32 accessMask)
        {
            ProcessId = processId;
            Type = type;
            Handle = handle;
            AccessMask = accessMask;
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
                // TODO: Need to find a better way for this
                Process owner = Process.Open(ProcessId, ProcessRights.DuplicateHandle);
                SafeProcessHandle duplicate = Edo.Win32.Handle.DuplicateProcessHandle(owner.Handle, Handle, ProcessRights.QueryInformation,
                    false, DuplicationOptions.None);

                return Convert.ToInt32(Kernel32.GetProcessId(duplicate.DangerousGetHandle())) == targetId;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether this handle holds at least given standard rights
        /// </summary>
        /// <param name="rights">The minimum access rights required</param>
        /// <returns>A boolean indicating whether this handle holds at minimum given access rights</returns>
        public Boolean HasRights(UInt32 rights)
        {
            return (rights & AccessMask) == rights;
        }

        /// <summary>
        /// Returns whether this handle holds at least given standard rights
        /// </summary>
        /// <param name="rights">The minimum access rights required</param>
        /// <returns>A boolean indicating whether this handle holds at minimum given access rights</returns>
        public Boolean HasRights(StandardRights rights)
        {
            return HasRights((uint)rights);
        }

        /// <summary>
        /// Returns whether this handle holds at least given process rights
        /// </summary>
        /// <param name="rights">The minimum access rights required</param>
        /// <returns>A boolean indicating whether this handle holds at minimum given access rights</returns>
        public Boolean HasRights(ProcessRights rights)
        {
            if(Type != HandleType.Process)
                throw new InvalidOperationException("Can only check process rights on a process handle");

            return HasRights((uint)rights);
        }

        /// <summary>
        /// The id of the process that owns this handle
        /// </summary>
        public Int32 ProcessId { get; }

        /// <summary>
        /// The type of this handle
        /// </summary>
        public HandleType Type { get; }

        /// <summary>
        /// A handle represented as an IntPtr
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// The access rights held by this handle in the context of its target process
        /// </summary>
        public UInt32 AccessMask { get; }
    }
}