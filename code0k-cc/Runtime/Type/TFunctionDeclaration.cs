using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using code0k_cc.Parse;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TFunctionDeclaration : IType
    {
        public override string TypeCodeName => "__Function";

        public ParseInstance Instance;
        public string FunctionName;
        public TType ReturnType;
        public TFunctionDeclarationArguments Arguments;
         
    }
}
