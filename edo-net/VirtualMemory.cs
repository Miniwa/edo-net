using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Edo.Native;
using Microsoft.Win32.SafeHandles;

namespace Edo
{
    /// <summary>
    /// Provides a managed low level interface to a target virtual memory
    /// </summary>
    public class VirtualMemory
    {
        public VirtualMemory()
        {
            ProcessHandle = null;
            Buffer = new byte[16];
        }

        /// <summary>
        /// Opens and targets the virtual memory of process with given id
        /// </summary>
        /// <param name="id">The id of the process to be opened</param>
        /// <exception cref="InvalidOperationException">If a target is already open</exception>
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
        /// <exception cref="InvalidOperationException">If a target is already open</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public void Open(Process process)
        {
            if(process == null)
                throw new ArgumentNullException(nameof(process));

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
        /// Whether a virtual memory has been opened or not
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
        public void Read(IntPtr address, byte[] buffer, Int32 count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

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
        public void Read(IntPtr address, Stream outStream, Int32 count)
        {
            if(outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            if (count > Buffer.Length)
                Buffer = new byte[count];

            Read(address, Buffer, count);
            outStream.Write(Buffer, 0, count);
        }

        /// <summary>
        /// Reads a structure or an instance of a formatted class from given address in target virtual memory
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be read</typeparam>
        /// <param name="address">The address to be read from</param>
        /// <returns>The structure or instance read from virtual memory</returns>
        /// <exception cref="InvalidOperationException">If no virtual memory has been targeted</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        public T Read<T>(IntPtr address)
        {
            int size = Marshal.SizeOf<T>();
            if(Buffer.Length < size)
                Buffer = new byte[size];

            Read(address, Buffer, size);
            return Seriz.Parse<T>(Buffer);
        }

        /// <summary>
        /// Writes given count of bytes from given buffer to given address in target virtual memory
        /// </summary>
        /// <param name="address">The address to be written to</param>
        /// <param name="buffer">The buffer containing the data to be written</param>
        /// <param name="count">The amount of bytes to write from the buffer</param>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="InvalidOperationException">If no virtual memory has been targeted</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write(IntPtr address, byte[] buffer, Int32 count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (count > buffer.Length)
                throw new ArgumentException("Not enough room in buffer to hold requested amount of data");

            if (!IsOpen)
                throw new InvalidOperationException("A virtual memory must be targeted before write operations are available");

            int nrBytesWritten = 0;
            if(!WinApi.WriteProcessMemory(ProcessHandle.DangerousGetHandle(), address, buffer, count, ref nrBytesWritten))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not perform write operation");

            if(nrBytesWritten != count)
                throw new InvalidOperationException(string.Format("Operation only wrote {0} out of {1} wanted bytes", nrBytesWritten, count));
        }

        /// <summary>
        /// Writes given count of bytes from given stream to given address in target virtual memory
        /// </summary>
        /// <param name="address">The address to be written to</param>
        /// <param name="stream">The stream containing the data to be written</param>
        /// <param name="count">The amount of bytes to write from the stream</param>
        /// <exception cref="InvalidOperationException">If no virtual memory has been targeted</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write(IntPtr address, Stream stream, Int32 count)
        {
            if(stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (count > Buffer.Length)
                Buffer = new byte[count];

            stream.Read(Buffer, 0, count);
            Write(address, Buffer, count);
        }

        /// <summary>
        /// Writes a structure or an instance of a formatted class to given address in target virtual memory
        /// </summary>
        /// <typeparam name="T">The type of the structure or formatted class to be written</typeparam>
        /// <param name="address">The address to be written to</param>
        /// <param name="value">The structure or instance of a formatted class to be written</param>
        public void Write<T>(IntPtr address, T value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            int size = Marshal.SizeOf(value);
            if (Buffer.Length < size)
                Buffer = new byte[size];
            
            Seriz.Serialize<T>(value, Buffer);
            Write(address, Buffer, size);
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
