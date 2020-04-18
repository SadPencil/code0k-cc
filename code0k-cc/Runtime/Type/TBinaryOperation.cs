using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TBinaryOperation : IType
    {
        public string TypeCodeName => "__BinaryOperation";
        public IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }
        public UnaryOperation Operation;
    }
}
