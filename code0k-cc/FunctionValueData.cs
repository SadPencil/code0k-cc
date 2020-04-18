using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class FunctionValueData:RuntimeValueData
    {
        public ParseInstance Instance;
        public string FunctionName;
        public RuntimeType ReturnType;
        public List<RuntimeType> ArgumentTypes;
    }
}
