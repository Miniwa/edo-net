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
            Marshal = null;
            ClosedMarshal = null;
            OutStream = new MemoryStream(32);
            Reader = new BinaryReader(OutStream);
        }

        [TestInitialize]
        public void Init()
        {
            Marshal = new VirtualMemory();
            Marshal.Open(Process.GetCurrentProcess());
            ClosedMarshal = new VirtualMemory();
            OutStream.Seek(0, SeekOrigin.Begin);
            OutStream.SetLength(0);
        }

        [TestMethod]
        public void TestInitialization()
        {
            Assert.IsFalse(ClosedMarshal.IsOpen);
        }

        [TestMethod]
        public void TestOpen()
        {
            ClosedMarshal.Open(Process.GetCurrentProcess());
            Assert.IsTrue(ClosedMarshal.IsOpen);
            Assert.IsNotNull(ClosedMarshal.ProcessHandle);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestOpenWhenAlreadyOpen()
        {
            ClosedMarshal.Open(Process.GetCurrentProcess());
            ClosedMarshal.Open(1);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestOpenThrowsOnApiError()
        {
            ClosedMarshal.Open(-1);
        }

        [TestMethod]
        public void TestClose()
        {
            ClosedMarshal.Open(Process.GetCurrentProcess());
            ClosedMarshal.Close();
            Assert.IsFalse(ClosedMarshal.IsOpen);
            Assert.IsNull(ClosedMarshal.ProcessHandle);
        }

        [TestMethod]
        public unsafe void TestRead()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            OutStream.SetLength(4);
            Marshal.Read(address, OutStream.GetBuffer(), 4);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        public unsafe void TestReadWithStream()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            Marshal.Read(address, OutStream, 4);
            OutStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReadThrowsIfBufferNotLargeEnough()
        {
            Marshal.Read(IntPtr.Zero, OutStream.GetBuffer(), 10000);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReadThrowsIfNotOpen()
        {
            ClosedMarshal.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestReadThrowsOnApiError()
        {
            Marshal.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        public VirtualMemory Marshal { get; set; }
        public VirtualMemory ClosedMarshal { get; set; }
        public MemoryStream OutStream { get; set; }
        public BinaryReader Reader { get; set; }
    }
}
