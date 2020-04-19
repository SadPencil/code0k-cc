using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime
{
    struct VariableRefRef
    {
        public VariableRef VariableRef;

        public VariableRefRef(VariableRef varRef)
        {
            this.VariableRef = varRef;
        }
    }
}
