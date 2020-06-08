using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace code0k_cc.Runtime.ValueOfType
{
    class NizkFieldValue : INizkValue
    {
        public bool IsConstant { get; set; } = true;
        public BigInteger Value;
    }
}
