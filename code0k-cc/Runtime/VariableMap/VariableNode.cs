using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.VariableMap
{
    class VariableNode : IVariableMapNode
    {
        public Variable Variable;

        public VariableNode(Variable variable)
        {
            this.Variable = variable;
        }
    }
}
