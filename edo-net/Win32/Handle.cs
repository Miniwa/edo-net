using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Edo.Win32.Native;

namespace Edo.Win32
{
    /// <summary>
    /// Provides a managed low-level interface to windows handles
    /// </summary>
    public class Handle
    {
        /// <summary>
        /// Returns a collection of handles that are currently active within the system
        /// </summary>
        /// <returns>A collection of active handles</returns>
        public static ICollection<HandleInfo> GetHandles()
        {
            byte[] buffer;
            NtStatus status;
            int size = 1024;
            uint actualSize = 0;
            do
            {
                buffer = new byte[size];
                status = NtDll.NtQuerySystemInformation(SystemInformationType.HandleInformation, buffer,
                    Convert.ToUInt32(size), ref actualSize);
                if (status != NtStatus.Success && status != NtStatus.BufferTooSmall &&
                    status != NtStatus.InfoLengthMismatch)
                    throw new Win32Exception("Could not retrieve system handle information");

                size = Convert.ToInt32(actualSize);
            } while (status != NtStatus.Success);

            int count = Seriz.Parse<int>(buffer);
            NtDll.SystemHandle[] systemHandles = Seriz.Parse<NtDll.SystemHandle>(buffer.Skip(4).ToArray(), count);
            List<HandleInfo> results = new List<HandleInfo>();
            foreach (var handle in systemHandles)
                results.Add(new HandleInfo(Convert.ToInt32(handle.ProcessId), handle.Type,
                    new IntPtr(handle.Handle), handle.AccessMask));

            return results;
        }

        /// <summary>
        /// Returns a collection of process handles that are currently active within the system
        /// </summary>
        /// <returns>A collection of active process handles</returns>
        public static ICollection<HandleInfo> GetProcessHandles()
        {
            return GetHandles().Where(handle => handle.Type == HandleType.Process).ToList();
        }

        /// <summary>
        /// Opens a handle with given access rights to a process with given process id
        /// </summary>
        /// <param name="id">The id of the process to be opened</param>
        /// <param name="desiredRights">The desired access rights to the process</param>
        /// <returns>The new handle to the process</returns>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public static SafeProcessHandle OpenProcess(Int32 id, ProcessRights desiredRights)
        {
            IntPtr handle = Kernel32.OpenProcess(desiredRights, false, Convert.ToUInt32(id));
            if(handle.IsNullPtr())
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not open handle to process");

            return new SafeProcessHandle(handle, true);
        }

        /// <summary>
        /// Duplicates a given handle owned by a given process into a given destination process
        /// </summary>
        /// <param name="owner">A handle to the process that owns the handle</param>
        /// <param name="sourceHandle">The handle to be duplicated</param>
        /// <param name="desiredRights">The desired rights of the new handle. Implementation specific</param>
        /// <param name="inherit">Whether the new handle is inheritable</param>
        /// <param name="options">The duplication options to use when duplicating</param>
        /// <param name="destination">The process that should recieve the handle duplicate</param>
        /// <returns>The newly duplicated handle</returns>
        /// <exception cref="Win32Exception">On windows api error</exception>
        public static IntPtr Duplicate(SafeProcessHandle owner, IntPtr sourceHandle,
            UInt32 desiredRights, Boolean inherit, DuplicationOptions options, SafeProcessHandle destination)
        {
            IntPtr destinationHandle = IntPtr.Zero;
            if(!Kernel32.DuplicateHandle(owner.DangerousGetHandle(), sourceHandle, destination.DangerousGetHandle(),
                ref destinationHandle, desiredRights, inherit, options))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not duplicate handle");
            }

            return destinationHandle;
        }

        /// <summary>
        /// Duplicates a given process handle owned by a given process into a given destination process
        /// </summary>
        /// <param name="owner">A handle to the process that owns the handle</param>
        /// <param name="sourceHandle">The process handle to be duplicated</param>
        /// <param name="desiredRights">The desired process rights of the new handle</param>
        /// <param name="inherit">Whether the new handle is inheritable</param>
        /// <param name="options">The duplication options to use when duplicating</param>
        /// <param name="destination">The process that should recieve the handle duplicate</param>
        /// <returns>The newly duplicated handle</returns>
        /// <exception cref="Win32Exception">On windows api error</exception>
        public static SafeProcessHandle DuplicateProcessHandle(SafeProcessHandle owner, IntPtr sourceHandle,
            ProcessRights desiredRights, Boolean inherit, DuplicationOptions options, SafeProcessHandle destination)
        {
            IntPtr handle = Duplicate(owner, sourceHandle,
                (UInt32)desiredRights, inherit, options, destination);

            return new SafeProcessHandle(handle, true);
        }

        /// <summary>
        /// Duplicates a given process handle owned by a given process into this process
        /// </summary>
        /// <param name="owner">A handle to the process that owns the handle</param>
        /// <param name="sourceHandle">The process handle to be duplicated</param>
        /// <param name="desiredRights">The desired process rights of the new handle</param>
        /// <param name="inherit">Whether the new handle is inheritable</param>
        /// <param name="options">The duplication options to use when duplicating</param>
        /// <returns>The newly duplicated handle</returns>
        /// <exception cref="Win32Exception">On windows api error</exception>
        public static SafeProcessHandle DuplicateProcessHandle(SafeProcessHandle owner, IntPtr sourceHandle,
            ProcessRights desiredRights, Boolean inherit, DuplicationOptions options)
        {
            var process = Process.GetCurrentProcess();
            return DuplicateProcessHandle(owner, sourceHandle, desiredRights,
                inherit, options, process.Handle);
        }

        /// <summary>
        /// Closes given handle
        /// </summary>
        /// <param name="handle">The handle to be closed</param>
        /// <exception cref="Win32Exception">On windows api error</exception>
        public static void Close(IntPtr handle)
        {
            if(!Kernel32.CloseHandle(handle))
                throw new Win32Exception("Could not close handle");
        }

        /// <summary>
        /// Returns an IntPtr that represents an invalid handle
        /// </summary>
        public static IntPtr Invalid => new IntPtr(Constants.InvalidHandleValue);
    }
}