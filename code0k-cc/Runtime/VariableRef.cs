using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime
{
    struct VariableRef
    {
        public Variable Variable;

        public VariableRef(Variable var)
        {
            this.Variable = var;
        }
    }
}
