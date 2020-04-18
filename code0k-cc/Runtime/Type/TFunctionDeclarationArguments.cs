using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TFunctionDeclarationArguments : IType
    {
        public override string TypeCodeName => "__TFunctionDeclarationArguments"; 

        public List<(TTypeOfType Type, string VarName)> Arguments = new List<(TTypeOfType Type, string VarName)>();
    }
}
