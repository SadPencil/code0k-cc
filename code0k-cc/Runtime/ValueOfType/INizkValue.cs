using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Nizk;

namespace code0k_cc.Runtime.ValueOfType
{
    abstract class INizkValue : IValueOfType
    {
        public Variable TagVariable { get; set; }
        public NizkVariableType VariableType { get; set; }
    }
}
