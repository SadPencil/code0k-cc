using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class FunctionExecuteArg:RuntimeTypeExecuteArg
    {
        public List<(string, RuntimeValue)> NameValues = new List<(string, RuntimeValue)>();
    }
}
