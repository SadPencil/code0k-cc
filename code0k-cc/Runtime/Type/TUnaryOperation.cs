using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TUnaryOperation : IType
    {
        public override string TypeCodeName => "__UnaryOperation";

        public UnaryOperation Operation;

    }
}
