using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Edo.Win32.Native;

namespace Edo.Win32
{
    /// <summary>
    /// Provides a managed low-level interface to windows threads
    /// </summary>
    public class Thread
    {
        /// <summary>
        /// Returns a collection of threads that are currently active in the system
        /// </summary>
        /// <returns></returns>
        public static ICollection<ThreadInfo> GetThreads()
        {
            IntPtr snapshot = Kernel32.CreateToolhelp32Snapshot(SnapshotFlags.Thread | SnapshotFlags.NoHeaps, 0);
            if(snapshot == Handle.Invalid)
                throw new Win32Exception(Marshal.GetLastWin32Error(),"Could not create thread snapshot");

            try
            {
                TlHelp32.ThreadEntry32 threadEntry = new TlHelp32.ThreadEntry32
                {
                    StructSize = Convert.ToUInt32(Marshal.SizeOf<TlHelp32.ThreadEntry32>())
                };

                if(!Kernel32.Thread32First(snapshot, ref threadEntry))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not load the first thread from the snapshot");

                List<ThreadInfo> threadList = new List<ThreadInfo>();
                do
                {
                    ThreadInfo info = new ThreadInfo(Convert.ToInt32(threadEntry.ThreadId),
                        Convert.ToInt32(threadEntry.ProcessId), threadEntry.Priority);
                    threadList.Add(info);
                }
                while(Kernel32.Thread32Next(snapshot, ref threadEntry));

                int code = Marshal.GetLastWin32Error();
                if(code != (int)ErrorCode.NoMoreFiles)
                    throw new Win32Exception(code, "Could not load the next thread from the snapshot");

                return threadList;
            }
            finally
            {
                Handle.Close(snapshot);
            }
        }
    }
}