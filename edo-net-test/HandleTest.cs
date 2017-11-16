using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edo.Win32;
using Edo.Win32.Native;

namespace Edo
{
    [TestClass]
    public class HandleTest
    {
        public HandleTest()
        {
            Current = Process.GetCurrentProcess();
        }

        [TestMethod]
        public void TestGetHandles()
        {
            var handles = Handle.GetProcessHandles()
                .Where(handle => handle.HasRights(ProcessRights.AllAccess) &&
                                 handle.TargetsProcess(Current.Id)).ToList();

            // Should be atleast one, as Current holds a handle to this process
            Assert.IsTrue(handles.Count > 0);
        }

        [TestMethod]
        public void TestOpenHandle()
        {
            using(var handle = Handle.OpenProcess(Current.Id, ProcessRights.AllAccess))
            {
                Assert.IsTrue(new Process(handle).Id == Current.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestOpenHandleThrowsOnApiError()
        {
            Handle.OpenProcess(0, ProcessRights.AllAccess);
        }

        [TestMethod]
        public void TestDuplicateHandle()
        {
            var duplicated = Handle.DuplicateProcessHandle(Current.Handle, Current.Handle.DangerousGetHandle(),
                ProcessRights.None, false, DuplicationOptions.SameAccess);

            Assert.AreNotEqual(Current.Handle.DangerousGetHandle(), duplicated.DangerousGetHandle());
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestDuplicateHandleThrowsOnApiError()
        {
            Handle.DuplicateHandle(Current.Handle, IntPtr.Zero, 0, false, DuplicationOptions.None, Current.Handle);
        }

        [TestMethod]
        public void TestInvalid()
        {
            Assert.AreEqual(new IntPtr(Constants.InvalidHandleValue), Handle.Invalid);
        }

        public Process Current { get; }
    }
}