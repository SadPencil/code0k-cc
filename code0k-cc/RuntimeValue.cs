using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class RuntimeValue
    {
        public RuntimeType Type;
        public object Data;

        public RuntimeValue Execute(EnvironmentBlock block, object arg)
        {
            return this.Type.Execute(block, this, arg);
        }

        
    }
}
