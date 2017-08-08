using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        public IntPtr Ptr { get; set; }
        public IntPtr NullPtr { get; set; }
    }
}