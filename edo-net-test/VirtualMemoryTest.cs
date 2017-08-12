using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edo.Mock;

namespace Edo
{
    [TestClass]
    public class VirtualMemoryTest
    {
        public VirtualMemoryTest()
        {
            Memory = null;
            ClosedMemory = null;
            OutStream = new MemoryStream(32);
            Reader = new BinaryReader(OutStream);
            Writer = new BinaryWriter(OutStream);
        }

        [TestInitialize]
        public void Init()
        {
            Memory = new VirtualMemory();
            Memory.Open(Process.GetCurrentProcess());
            ClosedMemory = new VirtualMemory();
            OutStream.Seek(0, SeekOrigin.Begin);
            OutStream.SetLength(0);
        }

        [TestMethod]
        public void TestInitialization()
        {
            Assert.IsFalse(ClosedMemory.IsOpen);
        }

        [TestMethod]
        public void TestOpen()
        {
            ClosedMemory.Open(Process.GetCurrentProcess());
            Assert.IsTrue(ClosedMemory.IsOpen);
            Assert.IsNotNull(ClosedMemory.ProcessHandle);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOpenThrowsOnNull()
        {
            ClosedMemory.Open(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestOpenWhenAlreadyOpen()
        {
            ClosedMemory.Open(Process.GetCurrentProcess());
            ClosedMemory.Open(1);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestOpenThrowsOnApiError()
        {
            ClosedMemory.Open(-1);
        }

        [TestMethod]
        public void TestClose()
        {
            ClosedMemory.Open(Process.GetCurrentProcess());
            ClosedMemory.Close();
            Assert.IsFalse(ClosedMemory.IsOpen);
            Assert.IsNull(ClosedMemory.ProcessHandle);
        }

        [TestMethod]
        public unsafe void TestRead()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            OutStream.SetLength(4);
            Memory.Read(address, OutStream.GetBuffer(), 4);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        public unsafe void TestReadWithStream()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            Memory.Read(address, OutStream, 4);
            OutStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        public unsafe void TestReadTWithFalseBooleanArgument()
        {
            Boolean value = false;
            IntPtr address = (IntPtr)(&value);

            Boolean result = Memory.Read<Boolean>(address);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public unsafe void TestReadTWithTrueBooleanArgument()
        {
            Boolean value = true;
            IntPtr address = (IntPtr)(&value);

            Boolean result = Memory.Read<Boolean>(address);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public unsafe void TestReadTWithInt32Argument()
        {
            Int32 number = 33123;
            IntPtr address = (IntPtr)(&number);

            Int32 result = Memory.Read<Int32>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithInt64Argument()
        {
            Int64 number = 331123222223;
            IntPtr address = (IntPtr)(&number);

            Int64 result = Memory.Read<Int64>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithSinglePrecisionArgument()
        {
            Single number = 1083.123123f;
            IntPtr address = (IntPtr)(&number);

            Single result = Memory.Read<Single>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithDoublePrecisionArgument()
        {
            Double number = 331123222223.123123123f;
            IntPtr address = (IntPtr)(&number);

            Double result = Memory.Read<Double>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithPointerArgument()
        {
            Double number = 331123222223.123123123f;
            IntPtr address = (IntPtr)(&number);
            IntPtr ptrAddress = (IntPtr)(&address);

            IntPtr result = Memory.Read<IntPtr>(ptrAddress);
            Assert.AreEqual(address, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithStructureArgument()
        {
            MockVector vector = new MockVector();
            vector.X = 123012.1023123f;
            vector.Y = 123123.123123f;
            vector.Z = 1222.1123123f;

            IntPtr address = (IntPtr)(&vector);
            MockVector result = Memory.Read<MockVector>(address);

            Assert.AreEqual(vector.X, result.X);
            Assert.AreEqual(vector.Y, result.Y);
            Assert.AreEqual(vector.Z, result.Z);
        }

        [TestMethod]
        public void TestReadTWithFormattedClassArgument()
        {
            MockClass test = new MockClass();
            test.value1 = 10231;
            test.value2 = 123123.123120f;
            test.value3 = 1232.123123f;
            test.value4 = 1231203112;
            test.value5 = 1031231231230123;

            int size = Marshal.SizeOf(test);
            IntPtr address = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(test, address, false);

                MockClass result = Memory.Read<MockClass>(address);
                Assert.AreEqual(test.value1, result.value1);
                Assert.AreEqual(test.value2, result.value2);
                Assert.AreEqual(test.value3, result.value3);
                Assert.AreEqual(test.value4, result.value4);
                Assert.AreEqual(test.value5, result.value5);
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadThrowsOnNull()
        {
            byte[] buffer = null;
            Memory.Read(IntPtr.Zero, buffer, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReadThrowsIfBufferNotLargeEnough()
        {
            Memory.Read(IntPtr.Zero, OutStream.GetBuffer(), 10000);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReadThrowsIfNotOpen()
        {
            ClosedMemory.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestReadThrowsOnApiError()
        {
            Memory.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadWithStreamThrowsOnNull()
        {
            Stream stream = null;
            Memory.Read(IntPtr.Zero, stream, 4);
        }

        [TestMethod]
        public unsafe void TestWrite()
        {
            int number = 30;
            Writer.Write(number);

            int result = 0;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, OutStream.GetBuffer(), 4);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteWithStream()
        {
            int number = 30;
            Writer.Write(number);

            int result = 0;
            IntPtr address = (IntPtr)(&result);

            OutStream.Seek(0, SeekOrigin.Begin);
            Memory.Write(address, OutStream, 4);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithFalseBooleanArgument()
        {
            Boolean result = true;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithTrueBooleanArgument()
        {
            Boolean result = false;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, true);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithInt32Argument()
        {
            Int32 number = 30123;

            Int32 result = 0;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithInt64Argument()
        {
            Int64 number = 30123123230023;

            Int64 result = 0;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithSingleArgument()
        {
            Single number = 301.123898f;

            Single result = 0;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithDoubleArgument()
        {
            Double number = 301.123898f;

            Double result = 0;
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithPointerArgument()
        {
            IntPtr number = new IntPtr(10000);
            IntPtr address = (IntPtr)(&number);
            IntPtr addressPtr = (IntPtr)(&address);

            Memory.Write(addressPtr, number);
            Assert.AreEqual(number, address);
        }

        [TestMethod]
        public unsafe void TestWriteTWithStructureArgument()
        {
            MockVector vector = new MockVector();
            vector.X = 123123.012312f;
            vector.Y = 123123.1235f;
            vector.Z = 1231.11111f;

            MockVector result = new MockVector();
            IntPtr address = (IntPtr)(&result);

            Memory.Write(address, vector);
            Assert.AreEqual(vector.X, result.X);
            Assert.AreEqual(vector.Y, result.Y);
            Assert.AreEqual(vector.Z, result.Z);
        }

        [TestMethod]
        public unsafe void TestWriteTWithFormattedClassArgument()
        {
            MockClass test = new MockClass();
            test.value1 = 10231;
            test.value2 = 123123.123120f;
            test.value3 = 1232.123123f;
            test.value4 = 1231203112;
            test.value5 = 1031231231230123;

            int size = Marshal.SizeOf(test);
            IntPtr address = Marshal.AllocHGlobal(size);
            try
            {
                Memory.Write(address, test);
                MockClass result = Marshal.PtrToStructure<MockClass>(address);

                Assert.AreEqual(test.value1, result.value1);
                Assert.AreEqual(test.value2, result.value2);
                Assert.AreEqual(test.value3, result.value3);
                Assert.AreEqual(test.value4, result.value4);
                Assert.AreEqual(test.value5, result.value5);
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteThrowsOnNull()
        {
            byte[] buffer = null;
            Memory.Write(IntPtr.Zero, buffer, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestWriteThrowsIfBufferNotLargeEnough()
        {
            Memory.Write(IntPtr.Zero, new byte[4], 5);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestWriteThrowsIfNotOpen()
        {
            ClosedMemory.Write(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestWriteThrowsOnApiError()
        {
            Memory.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteWithStreamThrowsOnNull()
        {
            Stream stream = null;
            Memory.Write(IntPtr.Zero, stream, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteTThrowsOnNull()
        {
            Memory.Write<MockClass>(IntPtr.Zero, null);
        }

        public VirtualMemory Memory { get; set; }
        public VirtualMemory ClosedMemory { get; set; }
        public MemoryStream OutStream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
    }
}
