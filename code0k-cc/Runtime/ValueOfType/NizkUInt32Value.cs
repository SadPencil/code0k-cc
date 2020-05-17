using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Nizk;

namespace code0k_cc.Runtime.ValueOfType
{
    sealed class NizkUInt32Value : INizkValue
    {
        public bool IsConstant { get; set; } = true;
        public UInt32 Value; 
    }
}
