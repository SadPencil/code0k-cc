using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Type;

namespace code0k_cc
{
    class ParseInstance
    {
        public ParseUnit ParseUnit = null;
        public Token Token = null;
        public IReadOnlyList<ParseInstance> Children;

        public IType Execute(EnvironmentBlock block,  FunctionExecuteArg arg)
        {
            return this.ParseUnit.Execute(this, block, arg);
        }
    }
}
