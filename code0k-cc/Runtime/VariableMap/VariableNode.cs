using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.VariableMap
{
    class VariableNode : IVariableMapNode
    {
        public RawVariable RawVariable;
        public NizkVariableType NizkAttribute { get; set; } = NizkVariableType.Intermediate;
        public string VarName { get; set; }

        public VariableNode(RawVariable rawVariable)
        {
            this.RawVariable = rawVariable;
        }
        public VariableNode(Variable variable) : this(variable.RawVariable) { }

        public VariableNode(VariableRef variableRef) : this(variableRef.Variable)
        {
            this.VarName = variableRef.VarName;
            this.NizkAttribute = variableRef.NizkAttribute;
        }
    }
}
