using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TFunctionInvokeParameters : IType
    {
        public override string TypeCodeName => "__TFunctionInvokeArguments"; 

        public List<(IType Value, string VarName)> Parameters = new List<(IType Value, string VarName)>();
    }
}
