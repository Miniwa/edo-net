using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
