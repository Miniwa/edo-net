using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edo.Mock;
using Edo.Win32;
using Edo.Win32.Native;

namespace Edo
{
    [TestClass]
    public class ProcessTest
    {
        public ProcessTest()
        {
            Proc = null;
            OutStream = new MemoryStream(32);
            Reader = new BinaryReader(OutStream);
            Writer = new BinaryWriter(OutStream);
        }

        [TestInitialize]
        public void Init()
        {
            Proc = Process.GetCurrentProcess();
            Id = Proc.Id;
            OutStream.Seek(0, SeekOrigin.Begin);
            OutStream.SetLength(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestOpenThrowsOnApiError()
        {
            Win32.Process.Open(0, ProcessRights.AllAccess);
        }

        [TestMethod]
        public void TestId()
        {
            int id = System.Diagnostics.Process.GetCurrentProcess().Id;
            Assert.AreEqual(id, Proc.Id);
        }

        [TestMethod]
        public void TestFileName()
        {
            string name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            Assert.AreEqual(name, Proc.FileName);
        }

        [TestMethod]
        public void TestModules()
        {
            var modules = System.Diagnostics.Process.GetCurrentProcess().Modules;
            var results = Proc.Modules;

            Assert.AreEqual(modules.Count, results.Count);
            foreach (System.Diagnostics.ProcessModule module in modules)
            {
                var match = results.Single(mod => module.FileName == mod.FullPath);
                Assert.AreEqual(module.ModuleName, match.FileName);
                Assert.AreEqual(module.BaseAddress, match.BaseAddress);
                Assert.AreEqual(module.ModuleMemorySize, match.BaseSize);
            }
        }

        [TestMethod]
        public void TestMainModule()
        {
            Module main = Proc.MainModule;
            var actual = System.Diagnostics.Process.GetCurrentProcess().MainModule;

            Assert.AreEqual(actual.BaseAddress, main.BaseAddress);
        }

        [TestMethod]
        public unsafe void TestRead()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            OutStream.SetLength(4);
            Proc.Read(address, OutStream.GetBuffer(), 4);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        public unsafe void TestReadWithStream()
        {
            int number = 30;
            IntPtr address = (IntPtr)(&number);

            Proc.Read(address, OutStream, 4);
            OutStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        public unsafe void TestReadTWithFalseBooleanArgument()
        {
            Boolean value = false;
            IntPtr address = (IntPtr)(&value);

            Boolean result = Proc.Read<Boolean>(address);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public unsafe void TestReadTWithTrueBooleanArgument()
        {
            Boolean value = true;
            IntPtr address = (IntPtr)(&value);

            Boolean result = Proc.Read<Boolean>(address);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public unsafe void TestReadTWithInt32Argument()
        {
            Int32 number = 33123;
            IntPtr address = (IntPtr)(&number);

            Int32 result = Proc.Read<Int32>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithInt64Argument()
        {
            Int64 number = 331123222223;
            IntPtr address = (IntPtr)(&number);

            Int64 result = Proc.Read<Int64>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithSinglePrecisionArgument()
        {
            Single number = 1083.123123f;
            IntPtr address = (IntPtr)(&number);

            Single result = Proc.Read<Single>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithDoublePrecisionArgument()
        {
            Double number = 331123222223.123123123f;
            IntPtr address = (IntPtr)(&number);

            Double result = Proc.Read<Double>(address);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestReadTWithPointerArgument()
        {
            Double number = 331123222223.123123123f;
            IntPtr address = (IntPtr)(&number);
            IntPtr ptrAddress = (IntPtr)(&address);

            IntPtr result = Proc.Read<IntPtr>(ptrAddress);
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
            MockVector result = Proc.Read<MockVector>(address);

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

                MockClass result = Proc.Read<MockClass>(address);
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
            Proc.Read(IntPtr.Zero, buffer, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReadThrowsIfBufferNotLargeEnough()
        {
            Proc.Read(IntPtr.Zero, OutStream.GetBuffer(), 10000);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestReadThrowsOnApiError()
        {
            Proc.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadWithStreamThrowsOnNull()
        {
            Stream stream = null;
            Proc.Read(IntPtr.Zero, stream, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReadTArrayThrowsOnZeroNegativeElementCount()
        {
            Proc.Read<Int32>(IntPtr.Zero, 0);
        }

        [TestMethod]
        public unsafe void TestWrite()
        {
            int number = 30;
            Writer.Write(number);

            int result = 0;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, OutStream.GetBuffer(), 4);
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
            Proc.Write(address, OutStream, 4);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithFalseBooleanArgument()
        {
            Boolean result = true;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithTrueBooleanArgument()
        {
            Boolean result = false;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, true);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithInt32Argument()
        {
            Int32 number = 30123;

            Int32 result = 0;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithInt64Argument()
        {
            Int64 number = 30123123230023;

            Int64 result = 0;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithSingleArgument()
        {
            Single number = 301.123898f;

            Single result = 0;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithDoubleArgument()
        {
            Double number = 301.123898f;

            Double result = 0;
            IntPtr address = (IntPtr)(&result);

            Proc.Write(address, number);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public unsafe void TestWriteTWithPointerArgument()
        {
            IntPtr number = new IntPtr(10000);
            IntPtr address = (IntPtr)(&number);
            IntPtr addressPtr = (IntPtr)(&address);

            Proc.Write(addressPtr, number);
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

            Proc.Write(address, vector);
            Assert.AreEqual(vector.X, result.X);
            Assert.AreEqual(vector.Y, result.Y);
            Assert.AreEqual(vector.Z, result.Z);
        }

        [TestMethod]
        public void TestWriteTWithFormattedClassArgument()
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
                Proc.Write(address, test);
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
            Proc.Write(IntPtr.Zero, buffer, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestWriteThrowsIfBufferNotLargeEnough()
        {
            Proc.Write(IntPtr.Zero, new byte[4], 5);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestWriteThrowsOnApiError()
        {
            Proc.Read(IntPtr.Zero, OutStream.GetBuffer(), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteWithStreamThrowsOnNull()
        {
            Stream stream = null;
            Proc.Write(IntPtr.Zero, stream, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteTThrowsOnNull()
        {
            MockClass value = null;
            Proc.Write<MockClass>(IntPtr.Zero, value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWriteTArrayThrowsOnNullArray()
        {
            Int32[] values = null;
            Proc.Write(IntPtr.Zero, values);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestWriteTArrayThrowsOnZeroArray()
        {
            Proc.Write<Int32>(IntPtr.Zero, new Int32[0]);
        }

        [TestMethod]
        public void TestWriteReadTArray()
        {
            Int32[] numbers = new int[3]
            {
                1235,
                5923,
                5818828
            };

            IntPtr address = Marshal.AllocHGlobal(numbers.Length * Marshal.SizeOf<Int32>());
            try
            {
                Proc.Write(address, numbers);
                Int32[] results = Proc.Read<Int32>(address, 3);

                Assert.AreEqual(numbers.Length, results.Length);
                Assert.AreEqual(numbers[0], results[0]);
                Assert.AreEqual(numbers[1], results[1]);
                Assert.AreEqual(numbers[2], results[2]);
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        [TestMethod]
        public void TestWriteReadTArrayStructure()
        {
            MockVector[] vectors = new MockVector[3]
            {
                new MockVector(12344.12323f, 177733.123f, 84717.1234f),
                new MockVector(1232444.12323f, 17237733.123f, 847317.1234f),
                new MockVector(1344.123f, 1733.123f, 717.1234f)
            };

            IntPtr address = Marshal.AllocHGlobal(vectors.Length * Marshal.SizeOf<MockVector>());
            try
            {
                Proc.Write(address, vectors);
                MockVector[] results = Proc.Read<MockVector>(address, 3);

                Assert.AreEqual(vectors.Length, results.Length);
                for (int i = 0; i < vectors.Length; i++)
                {
                    Assert.AreEqual(vectors[i].X, results[i].X);
                    Assert.AreEqual(vectors[i].Y, results[i].Y);
                    Assert.AreEqual(vectors[i].Z, results[i].Z);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        [TestMethod]
        public void TestWriteReadTArrayFormattedClass()
        {
            MockClass[] classes = new MockClass[3]
            {
                new MockClass(1823825882, 19242342.12312312f, 12312387.132123f, 919484882, 78787237472374),
                new MockClass(18382882, 192342.1231312f, 1212387.13123f, 9184882, 7878723742372374),
                new MockClass(18238252882, 192422342.123123132f, 123123827.1322123f, 91948882, 78787823472374)
            };

            IntPtr address = Marshal.AllocHGlobal(classes.Length * Marshal.SizeOf<MockClass>());
            try
            {
                Proc.Write(address, classes);
                MockClass[] results = Proc.Read<MockClass>(address, 3);

                Assert.AreEqual(classes.Length, results.Length);
                for (int i = 0; i < classes.Length; i++)
                {
                    Assert.AreEqual(classes[i].value1, results[i].value1);
                    Assert.AreEqual(classes[i].value2, results[i].value2);
                    Assert.AreEqual(classes[i].value3, results[i].value3);
                    Assert.AreEqual(classes[i].value4, results[i].value4);
                    Assert.AreEqual(classes[i].value5, results[i].value5);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(address);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAllocThrowsOnZeroCount()
        {
            Proc.Alloc(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestAllocThrowsOnApiError()
        {
            Proc.Alloc(int.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestFreeThrowsOnApiError()
        {
            Proc.Free(IntPtr.Zero);
        }

        [TestMethod]
        public void TestAllocFree()
        {
            IntPtr address = Proc.Alloc(1024);
            Proc.Free(address);
        }

        [TestMethod]
        public void TestSetPrivilege()
        {
            Proc.SetPrivilege(PrivilegeName.Debug, true);
            Proc.SetPrivilege(PrivilegeName.AuthenticateAsUser, true);
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestSetPrivilegeThrowsOnApiError()
        {
            Proc.SetPrivilege("testjaskdajsd", true);
        }

        public Int32 Id { get; set; } 
        public Process Proc { get; set; }
        public MemoryStream OutStream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
    }
}
