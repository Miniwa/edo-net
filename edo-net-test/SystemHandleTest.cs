using System;
using System.ComponentModel;
using Edo.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edo
{
    [TestClass]
    public class SystemHandleTest
    {
        public SystemHandleTest()
        {
            Current = Win32Process.GetCurrentProcess();
            LocalHandle = new SystemHandle(Current.Id, Current.Handle.DangerousGetHandle(),
                ProcessRights.QueryInformation);
            InvalidHandle = new SystemHandle(0, IntPtr.Zero, ProcessRights.None);
        }

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod]
        public void TestDuplicate()
        {
            var duplicated = LocalHandle.Duplicate(false);
            Assert.AreNotEqual(LocalHandle.Handle, duplicated.DangerousGetHandle());
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void TestDuplicateThrowsOnApiError()
        {
            InvalidHandle.Duplicate(false);
        }

        [TestMethod]
        public void TestTargetsProcess()
        {
            Assert.IsTrue(LocalHandle.TargetsProcess(Current.Id));
        }

        [TestMethod]
        public void TestTargetsProcessReturnsFalseOnApiError()
        {
            Assert.IsFalse(InvalidHandle.TargetsProcess(Current.Id));
        }

        [TestMethod]
        public void TestHasRights()
        {
            Assert.IsTrue(LocalHandle.HasRights(ProcessRights.None));
            Assert.IsTrue(LocalHandle.HasRights(ProcessRights.QueryInformation));
            Assert.IsFalse(LocalHandle.HasRights(ProcessRights.CreateThread));
        }

        public Win32Process Current { get; set; }
        public SystemHandle LocalHandle { get; set; }
        public SystemHandle InvalidHandle { get; set; }
        public SystemHandle ForeignHandle { get; set; }
    }
}