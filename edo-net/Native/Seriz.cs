using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Edo.Native
{
    /// <summary>
    /// Collection of methods for serializing to and from binary
    /// </summary>
    public static class Seriz
    {
        /// <summary>
        /// Serializes a given structure or instance of a formatted class to bytes and writes the bytes into given buffer
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be serialized</typeparam>
        /// <param name="buffer">The buffer to write the resulting bytes to</param>
        /// <param name="value">The value to ve serialized</param>
        /// <exception cref="ArgumentException">If buffer is too small to fit the data</exception>
        public static void Serialize<T>(byte[] buffer, T value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            int size = Marshal.SizeOf(value);
            if(size > buffer.Length)
                throw new ArgumentException("Buffer is too small to fit requested data");

            T[] values = new T[1];
            values[0] = value;
            Serialize(buffer, values);
        }

        /// <summary>
        /// Serializes an array of given structures or instances of a formatted class to bytes and writes the bytes into given buffer
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be serialized</typeparam>
        /// <param name="buffer">The buffer to write the resulting bytes to</param>
        /// <param name="values">The array of values to be serialized</param>
        /// <exception cref="ArgumentException">If buffer is too small to fit the data</exception>
        public static void Serialize<T>(byte[] buffer, T[] values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));
            
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(values.Length == 0)
                throw new ArgumentException("Cannot serialize an array of length zero");

            int size = Marshal.SizeOf<T>();
            int totalSize = size * values.Length;
            if(buffer.Length < totalSize)
                throw new ArgumentException("Buffer is too small to fit requested data");

            IntPtr address = Marshal.AllocHGlobal(totalSize);
            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Marshal.StructureToPtr(values[i], IntPtr.Add(address, i * size), false);
                }

                Marshal.Copy(address, buffer, 0, totalSize);
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        /// <summary>
        /// Parses a structure or instance of a formatted class from given buffer of bytes
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be parsed</typeparam>
        /// <param name="buffer">The buffer containing the bytes to be parsed</param>
        /// <returns>The parsed structure or instance of a formatted class</returns>
        /// <exception cref="ArgumentException">If the buffer is too small to fit the requested data</exception>
        public static T Parse<T>(byte[] buffer)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return Parse<T>(buffer, 1)[0];
        }

        /// <summary>
        /// Parses an array of structures or instances of a formatted class from given buffer of bytes
        /// </summary>
        /// <typeparam name="T">The type of structure or formatted class to be parsed</typeparam>
        /// <param name="buffer">The buffer containing the bytes to be parsed</param>
        /// <param name="count">The number of elements be parsed into the resulting array</param>
        /// <returns>The parsed array of structures or instances of a formatted class</returns>
        /// <exception cref="ArgumentException">If the buffer is too small to fit the requested data</exception>
        /// <exception cref="ArgumentException">If the element count is zero or a negative integer</exception>
        public static T[] Parse<T>(byte[] buffer, Int32 count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(count <= 0)
                throw new ArgumentException("Element count must be a positive integer");

            int size = Marshal.SizeOf<T>();
            int totalSize = size * count;
            if(buffer.Length < totalSize)
                throw new ArgumentException("Buffer too small to fit requested data");

            IntPtr address = Marshal.AllocHGlobal(totalSize);
            try
            {
                T[] values = new T[count];
                Marshal.Copy(buffer, 0, address, totalSize);
                for (int i = 0; i < count; i++)
                {
                    values[i] = Marshal.PtrToStructure<T>(IntPtr.Add(address, i * size));
                }

                return values;
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }
    }
}
