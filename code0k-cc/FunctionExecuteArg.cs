using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class FunctionExecuteArg:RuntimeTypeExecuteArg
    {
        public Dictionary<string, RuntimeValue> NameValues = new Dictionary<string, RuntimeValue>();
    }
}
