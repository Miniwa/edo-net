using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edo
{
    struct MockVector
    {
        public Single X;
        public Single Y;
        public Single Z;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    class MockClass
    {
        [FieldOffset(0)]
        public Int64 value1;

        [FieldOffset(8)]
        public Double value2;

        [FieldOffset(16)]
        public Single value3;

        [FieldOffset(20)]
        public UInt32 value4;

        [FieldOffset(24)]
        public UInt64 value5;
    }

    [TestClass]
    public class VirtualMemoryTest
    {
        public VirtualMemoryTest()
        {
            Memory = null;
            ClosedMemory = null;
            OutStream = new MemoryStream(32);
            Reader = new BinaryReader(OutStream);
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
        public unsafe void TestReadTWithFormattedClassArgument()
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

        public VirtualMemory Memory { get; set; }
        public VirtualMemory ClosedMemory { get; set; }
        public MemoryStream OutStream { get; set; }
        public BinaryReader Reader { get; set; }
    }
}
