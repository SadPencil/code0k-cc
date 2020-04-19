using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime.Operation
{
    class PropertyOperationDescription
    {
        public string PropertyName;
        public TType ReturnType;
        public TFunctionDeclarationArguments Arguments;
        public Func<EnvironmentBlock,FunctionExecuteArg, AssignExecuteArg, IType> Execute;
    }
}
