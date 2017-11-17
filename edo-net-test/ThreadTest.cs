
using System.Linq;
using Edo.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edo
{
    [TestClass]
    public class ThreadTest
    {
        public ThreadTest()
        {
            Current = Process.GetCurrentProcess();
        }

        [TestMethod]
        public void TestGetThreads()
        {
            var threads = Thread.GetThreads().Where(thread => thread.ProcessId == Current.Id);
            Assert.IsTrue(threads.Count() > 1);
        }

        public Process Current { get; }
    }
}
