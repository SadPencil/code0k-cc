using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TFunctionInvokeArguments : IType
    {
        public override string TypeCodeName => "__TFunctionInvokeArguments"; 

        public List<(IType Value, string VarName)> Arguments = new List<(IType Value, string VarName)>();
    }
}
