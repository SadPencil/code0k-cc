using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;

namespace code0k_cc.Runtime.Type
{
    interface IType
    {
        public abstract string TypeCodeName { get; }
        //public abstract IType Execute(EnvironmentBlock block, IType value, IRuntimeTypeExecuteArg arg);
        public abstract IType Execute(EnvironmentBlock block, IRuntimeTypeExecuteArg arg);
        public abstract bool ToBool();
        public abstract Int32 ToInt32();
    }
}
