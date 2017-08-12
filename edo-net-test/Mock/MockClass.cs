using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Edo.Mock
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public class MockClass
    {
        public MockClass()
        {
            
        }

        public MockClass(Int64 first, Double second, Single third, UInt32 fourth, UInt64 fifth)
        {
            value1 = first;
            value2 = second;
            value3 = third;
            value4 = fourth;
            value5 = fifth;
        }

        [FieldOffset(0)]
        public Int64 value1;

        [FieldOffset(8)]
        public Double value2;

        [FieldOffset(16)]
        public Single value3;

        [FieldOffset(20)]
        public UInt32 value4;

        [FieldOffset(24)]
        public UInt64 value5;
    }
}
