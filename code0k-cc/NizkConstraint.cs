using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class NizkConstraint : NizkNode
    {
        public NizkConstraintType Type { get; set; }
        public UInt64 Value { get; set; }
    }
}
