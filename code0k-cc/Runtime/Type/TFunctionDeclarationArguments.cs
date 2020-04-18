using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;

namespace code0k_cc.Runtime.Type
{
    class TFunctionDeclarationArguments : IType
    {
        public string TypeCodeName => "__TFunctionDeclarationArguments";
        public IType Execute(EnvironmentBlock block, IRuntimeTypeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }

        public List<(TTypeOfType Type, string VarName)> Arguments = new List<(TTypeOfType Type, string VarName)>();
    }
}
