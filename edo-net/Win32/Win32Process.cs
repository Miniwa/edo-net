using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Edo.Win32.Model;
using Microsoft.Win32.SafeHandles;

namespace Edo.Win32
{
    /// <summary>
    /// Provides a managed low level interface to a process
    /// </summary>
    public class Win32Process
    {
        /// <summary>
        /// Opens a handle with given access rights to a process with given process id
        /// </summary>
        /// <param name="id">The id of the process to be opened</param>
        /// <param name="desiredAccess">The desired access rights to the process</param>
        /// <returns>The new handle to the process</returns>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public static SafeProcessHandle OpenHandle(Int32 id, ProcessAccess desiredAccess)
        {
            IntPtr handle = Api.OpenProcess(desiredAccess, false, Convert.ToUInt32(id));
            if (handle.IsNullPtr())
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not open handle to process");

            return new SafeProcessHandle(handle, true);
        }

        /// <summary>
        /// Opens a process with given process id
        /// </summary>
        /// <param name="id">The id of the process to be opened</param>
        /// <param name="desiredAccess">The desired access rights to the process</param>
        /// <returns>The newly opened process</returns>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        public static Win32Process Open(Int32 id, ProcessAccess desiredAccess)
        {
            return new Win32Process(OpenHandle(id, desiredAccess));
        }

        /// <summary>
        /// Creates the process with given handle
        /// </summary>
        /// <param name="handle">A handle to the process to be targeted</param>
        public Win32Process(SafeProcessHandle handle)
        {
            Handle = handle;
            Buffer = new byte[16];
        }

        /// <summary>
        /// Reads given amount of bytes from given address in virtual memory of the process and writes them into given buffer
        /// </summary>
        /// <param name="address">The address to be read from</param>
        /// <param name="buffer">The buffer to write the data to</param>
        /// <param name="count">The number of bytes to be read</param>
        /// <exception cref="ArgumentException">If count is equal to or less than zero</exception>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        public void Read(IntPtr address, byte[] buffer, Int32 count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(count <= 0)
                throw new ArgumentException("Count must be greater than zero");

            if (count > buffer.Length)
                throw new ArgumentException("Not enough room in buffer to hold requested amount of data");

            UIntPtr nrBytesRead;
            UInt32 unsignedCount = Convert.ToUInt32(count);
            if (!Api.ReadProcessMemory(Handle.DangerousGetHandle(), address, buffer,
                new UIntPtr(unsignedCount), out nrBytesRead))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not perform read operation");

            if (nrBytesRead.ToUInt32() != unsignedCount)
                throw new InvalidOperationException(string.Format("Operation only read {0} out of {1} wanted bytes", nrBytesRead, count));
        }

        /// <summary>
        /// Reads given amount of bytes from given address in virtual memory of the process and writes them into given stream
        /// </summary>
        /// <param name="address">The address to be read from</param>
        /// <param name="outStream">The stream to write the data to</param>
        /// <param name="count">The amount of bytes to be read</param>
        /// <exception cref="ArgumentException">If count is equal to or less than zero</exception>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        public void Read(IntPtr address, Stream outStream, Int32 count)
        {
            if(outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            if(count <= 0)
                throw new ArgumentException("Count must be greater than zero");

            if (count > Buffer.Length)
                Buffer = new byte[count];

            Read(address, Buffer, count);
            outStream.Write(Buffer, 0, count);
        }

        /// <summary>
        /// Reads a structure or an instance of a formatted class from given address in virtual memory of the process
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be read</typeparam>
        /// <param name="address">The address to be read from</param>
        /// <returns>The structure or instance read from virtual memory of the process</returns>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        public T Read<T>(IntPtr address)
        {
            return Read<T>(address, 1)[0];
        }

        /// <summary>
        /// Reads an array of structures or instances of a formatted class from given address in virtual memory of the process
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be read</typeparam>
        /// <param name="address">The address to be read from</param>
        /// <param name="count">The number of elements in the array to be read</param>
        /// <returns>The array of structures or instances read from virtual memory of the process</returns>
        /// <exception cref="ArgumentException">If count is equal to or less than zero</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were read</exception>
        public T[] Read<T>(IntPtr address, Int32 count)
        {
            if(count <= 0)
                throw new ArgumentException("Count must be greater than zero");

            int size = Marshal.SizeOf<T>();
            int totalSize = size * count;
            if(Buffer.Length < totalSize)
                Buffer = new byte[totalSize];

            Read(address, Buffer, totalSize);
            return Seriz.Parse<T>(Buffer, count);
        }

        /// <summary>
        /// Writes given count of bytes from given buffer to given address in virtual memory of the process
        /// </summary>
        /// <param name="address">The address to be written to</param>
        /// <param name="buffer">The buffer containing the data to be written</param>
        /// <param name="count">The amount of bytes to write from the buffer</param>
        /// <exception cref="ArgumentException">If count is equal to or less than zero</exception>
        /// <exception cref="ArgumentException">If buffer is too small to fit the requested data</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write(IntPtr address, byte[] buffer, Int32 count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(count <= 0)
                throw new ArgumentException("Count must be greater than or equal to zero");

            if (count > buffer.Length)
                throw new ArgumentException("Not enough room in buffer to hold requested amount of data");

            UInt32 unsignedCount = Convert.ToUInt32(count);
            UIntPtr nrBytesWritten;
            if(!Api.WriteProcessMemory(Handle.DangerousGetHandle(), address, buffer,
                new UIntPtr(unsignedCount), out nrBytesWritten))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not perform write operation");

            if(nrBytesWritten.ToUInt32() != unsignedCount)
                throw new InvalidOperationException(string.Format("Operation only wrote {0} out of {1} wanted bytes", nrBytesWritten, count));
        }

        /// <summary>
        /// Writes given count of bytes from given stream to given address in virtual memory of the process
        /// </summary>
        /// <param name="address">The address to be written to</param>
        /// <param name="stream">The stream containing the data to be written</param>
        /// <param name="count">The amount of bytes to write from the stream</param>
        /// <exception cref="ArgumentException">If count is equal to or less than zero</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write(IntPtr address, Stream stream, Int32 count)
        {
            if(stream == null)
                throw new ArgumentNullException(nameof(stream));

            if(count <= 0)
                throw new ArgumentException("Count must be greater then zero");

            if (count > Buffer.Length)
                Buffer = new byte[count];

            stream.Read(Buffer, 0, count);
            Write(address, Buffer, count);
        }

        /// <summary>
        /// Writes a structure or an instance of a formatted class to given address in virtual memory of the process
        /// </summary>
        /// <typeparam name="T">The type of the structure or formatted class to be written</typeparam>
        /// <param name="address">The address to be written to</param>
        /// <param name="value">The structure or instance of a formatted class to be written</param>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write<T>(IntPtr address, T value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));
            
            Write(address, new T[] {value});
        }

        /// <summary>
        /// Writes an array of structures or instances of a formatted class to given address in virtual memory of the process
        /// </summary>
        /// <typeparam name="T">The type of the structure or formatted class to be written</typeparam>
        /// <param name="address">The address to be written to</param>
        /// <param name="values">The array of structures or instances of a formatted class to be written</param>
        /// <exception cref="ArgumentException">If the given array is empty</exception>
        /// <exception cref="Win32Exception">On Windows API error</exception>
        /// <exception cref="InvalidOperationException">If the call succeeds but too few bytes were written</exception>
        public void Write<T>(IntPtr address, T[] values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            if(values.Length == 0)
                throw new ArgumentException("Cannot write an empty array");

            int size = Marshal.SizeOf<T>();
            int totalSize = size * values.Length;
            if(Buffer.Length < totalSize)
                Buffer = new byte[totalSize];

            Seriz.Serialize(Buffer, values);
            Write(address, Buffer, totalSize);
        }

        /// <summary>
        /// The active handle to the process
        /// </summary>
        public SafeProcessHandle Handle { get; private set; }

        /// <summary>
        /// The id of the process
        /// </summary>
        public Int32 Id
        {
            get { return Convert.ToInt32(Api.GetProcessId(Handle.DangerousGetHandle())); }
        }

        /// <summary>
        /// The full path of the file used to start the process
        /// </summary>
        public string FullPath
        {
            get
            {
                uint capacity = 1024;
                StringBuilder builder = new StringBuilder(Convert.ToInt32(capacity));
                if(!Api.QueryFullProcessImageName(Handle.DangerousGetHandle(), 0, builder, ref capacity))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not retrieve process filename");

                return builder.ToString(0, Convert.ToInt32(capacity));
            }
        }

        /// <summary>
        /// The filename of the file used to start the process
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(FullPath); }
        }

        /// <summary>
        /// The collection of modules that are loaded into this process
        /// </summary>
        public ICollection<Module> Modules
        {
            get
            {
                IntPtr snapshot = Api.CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.NoHeaps, Convert.ToUInt32(Id));
                if(snapshot.IsInvalidHandle())
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not create module snapshot");

                try
                {
                    ModuleEntry32 moduleEntry = new ModuleEntry32();
                    moduleEntry.StructSize = Convert.ToUInt32(Marshal.SizeOf<ModuleEntry32>());
                    if (!Api.Module32First(snapshot, ref moduleEntry))
                        throw new Win32Exception(Marshal.GetLastWin32Error(),
                            "Could not load the first module from the snapshot");

                    List<Module> modules = new List<Module>();
                    do
                    {
                        Module module = new Module();
                        module.BaseAddress = moduleEntry.BaseAddress;
                        module.BaseSize = Convert.ToInt32(moduleEntry.BaseSize);
                        module.FullPath = moduleEntry.FullPath;

                        modules.Add(module);
                    }
                    while (Api.Module32Next(snapshot, ref moduleEntry));

                    int code = Marshal.GetLastWin32Error();
                    if(code != (int)ErrorCodes.NoMoreFiles)
                        throw new Win32Exception(code, "Could not load the next module from the snapshot");

                    return modules;
                }
                finally
                {
                    if(!Api.CloseHandle(snapshot))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not close snapshot handle");
                }
            }
        }

        /// <summary>
        /// The main module of the process
        /// </summary>
        public Module MainModule
        {
            get { return Modules.Single(module => module.FileName == FileName); }
        }

        /// <summary>
        /// Internal buffer used in some operations
        /// </summary>
        private byte[] Buffer { get; set; }
    }
}
