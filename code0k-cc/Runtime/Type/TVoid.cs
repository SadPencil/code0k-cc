using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;

namespace code0k_cc.Runtime.Type
{
    class TVoid : IType
    {
        public string TypeCodeName => "Void";
        public IType Execute(EnvironmentBlock block, IRuntimeTypeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() => false;
        public int ToInt32() => 0;
    }
}
