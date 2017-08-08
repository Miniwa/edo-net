using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Edo.Windows;
using Microsoft.Win32.SafeHandles;


namespace Edo
{
    /// <summary>
    /// Provides a managed low level interface to a target virtual memory
    /// </summary>
    public class VmMarshal
    {
        public VmMarshal()
        {
            ProcessHandle = null;
            Buffer = new byte[16];
        }

        /// <summary>
        /// Opens and targets the virtual memory of process with given id
        /// </summary>
        /// <param name="id">The id of the process to be opened</param>
        /// <exception cref="InvalidOperationException">If marshal already has a target open</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public void Open(Int32 id)
        {
            if(IsOpen)
                throw new InvalidOperationException("A virtual memory has already been targeted");

            IntPtr handle = WinApi.OpenProcess(WinApi.PROCESS_VM_ALL_ACCESS, false, id);
            if (handle.IsNullPtr())
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not open handle to process");

            ProcessHandle = new SafeProcessHandle(handle, true);
        }

        /// <summary>
        /// Opens and targets the virtual memory of given process
        /// </summary>
        /// <param name="process">The process to be opened</param>
        /// <exception cref="InvalidOperationException">If marshal already has a target open</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public void Open(Process process)
        {
            Open(process.Id);
        }

        /// <summary>
        /// Closes the currently targeted virtual memory
        /// </summary>
        public void Close()
        {
            if (IsOpen)
            {
                ProcessHandle.Close();
                ProcessHandle = null;
            }
        }

        /// <summary>
        /// Whether the marshal has opened a virtual memory or not
        /// </summary>
        public Boolean IsOpen
        {
            get { return ProcessHandle != null && !ProcessHandle.IsClosed; }
        }

        /// <summary>
        /// Reads given amount of bytes from given address in target virtual memory and writes them into given buffer
        /// </summary>
        /// <param name="address">The address to be read from</param>
        /// <param name="buffer">The buffer to write the data to</param>
        /// <param name="count">The number of bytes to be read</param>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="InvalidOperationException">If no virtual memory has been targeted</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        /// <returns></returns>
        public void Read(IntPtr address, byte[] buffer, Int32 count)
        {
            if (count > buffer.Length)
                throw new ArgumentException("Not enough room in buffer to hold requested amount of data");

            if (!IsOpen)
                throw new InvalidOperationException("A virtual memory must be targeted before read operations are available");

            int nrBytesRead = 0;
            if (!WinApi.ReadProcessMemory(ProcessHandle.DangerousGetHandle(), address, buffer, count, ref nrBytesRead))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not perform read operation");

            if (nrBytesRead != count)
                throw new InvalidOperationException(string.Format("Operation only read {0} out of {1} wanted bytes", nrBytesRead, count));
        }

        /// <summary>
        /// Reads given amount of bytes from given address in target virtual memory and writes them into given stream
        /// </summary>
        /// <param name="address">The address to be read from</param>
        /// <param name="outStream">The stream to write the data to</param>
        /// <param name="count">The amount of bytes to be read</param>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="InvalidOperationException">If no virtual memory has been targeted</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        /// <returns></returns>
        public void Read(IntPtr address, Stream outStream, Int32 count)
        {
            if (count > Buffer.Length)
                Buffer = new byte[count];

            Read(address, Buffer, count);
            outStream.Write(Buffer, 0, count);
        }

        /// <summary>
        /// Handle to the currently open process
        /// </summary>
        public SafeProcessHandle ProcessHandle { get; private set; }

        /// <summary>
        /// Internal buffer used in some operations
        /// </summary>
        private byte[] Buffer { get; set; }
    }
}
