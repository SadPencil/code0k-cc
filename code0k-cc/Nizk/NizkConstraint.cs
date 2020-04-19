using System;

namespace code0k_cc.Nizk
{
    class NizkConstraint : NizkNode
    {
        public NizkConstraintType Type { get; set; }
        public UInt64 Value { get; set; }
    }
}
