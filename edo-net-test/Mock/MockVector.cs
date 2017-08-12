using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edo.Mock
{
    public struct MockVector
    {
        public MockVector(Single x, Single y, Single z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Single X;
        public Single Y;
        public Single Z;
    }
}
