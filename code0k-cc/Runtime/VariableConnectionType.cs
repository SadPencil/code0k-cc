using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime
{
    class VariableConnectionType
    {
        // union; one of them has value, others == null
        public UnaryOperation UnaryOperation;
        public BinaryOperation BinaryOperation;
        public SpecialOperation SpecialOperation;
    }
}
