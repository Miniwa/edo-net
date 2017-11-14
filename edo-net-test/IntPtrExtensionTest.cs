using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edo.Win32.Native;

namespace Edo
{
    [TestClass]
    public class IntPtrExtensionTest
    {
        [TestInitialize]
        public void Init()
        {
            Ptr = new IntPtr(8000);
            NullPtr = new IntPtr(0);
            InvalidPtr = new IntPtr(Constant.InvalidHandleValue);
        }

        [TestMethod]
        public void TestIsNullPtrReturnsFalseIfNotNull()
        {
            Assert.IsFalse(Ptr.IsNullPtr());
        }

        [TestMethod]
        public void TestIsNullPtrReturnsTrueIfNull()
        {
            Assert.IsTrue(NullPtr.IsNullPtr());
        }

        [TestMethod]
        public void TestIsInvalidHandleReturnsFalseIfNotInvalid()
        {
            Assert.IsFalse(Ptr.IsInvalidHandle());
        }

        [TestMethod]
        public void TestIsInvalidHandleReturnsTrueIfInvalid()
        {
            Assert.IsTrue(InvalidPtr.IsInvalidHandle());
        }

        public IntPtr Ptr { get; set; }
        public IntPtr NullPtr { get; set; }
        public IntPtr InvalidPtr { get; set; }
    }
}