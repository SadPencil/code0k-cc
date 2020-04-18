using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class RuntimeValue
    {
        public RuntimeType Type;
        public RuntimeValueData Data;

        public RuntimeValue() { }
        public RuntimeValue Execute(EnvironmentBlock block, RuntimeTypeExecuteArg arg)
        {
            return this.Type.Execute(block, this, arg);
        }


    }
}
