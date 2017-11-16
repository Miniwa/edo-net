using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edo.Win32;
using Edo.Win32.Native;

namespace Edo
{
    [TestClass]
    public class HandleInfoTest
    {
        public HandleInfoTest()
        {
            Current = Process.GetCurrentProcess();
            LocalHandle = new HandleInfo(Current.Id, HandleType.Process, Current.Handle.DangerousGetHandle(),
                (uint)ProcessRights.QueryInformation);
            InvalidHandle = new HandleInfo(0, HandleType.None,  IntPtr.Zero, (uint)ProcessRights.None);
        }

        [TestInitialize]
        public void Initialize()
        {
            
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestHasRightsThrowsIfNotProcessHandle()
        {
            InvalidHandle.HasRights(ProcessRights.CreateThread);
        }

        public Process Current { get; set; }
        public HandleInfo LocalHandle { get; set; }
        public HandleInfo InvalidHandle { get; set; }
    }
}