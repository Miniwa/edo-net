using System;

namespace Edo
{
    /// <summary>
    /// Extends IntPtr class with additional methods
    /// </summary>
    public static class IntPtrExtension
    {
        /// <summary>
        /// Returns whether this IntPtr is a null pointer
        /// </summary>
        /// <param name="ptr">The pointer to be checked</param>
        /// <returns></returns>
        public static Boolean IsNullPtr(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }
    }
}