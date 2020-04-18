using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class ParseInstance
    {
        public ParseUnit ParseUnit = null;
        public Token Token = null;
        public IReadOnlyList<ParseInstance> Children;

        public RuntimeValue Execute(EnvironmentBlock block,  FunctionExecuteArg arg)
        {
            return this.ParseUnit.Execute(this, block, arg);
        }
    }
}
