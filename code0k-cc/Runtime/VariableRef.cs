using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Nizk;

namespace code0k_cc.Runtime
{
    class VariableRef : ICloneable
    {
        public Variable Variable { get; set; }

        public NizkVariableType NizkAttribute { get; set; } = NizkVariableType.Intermediate;

        public string VarName { get; set; }

        public VariableRef(Variable variable)
        {
            this.Variable = variable;
        }

        /// <summary>
        /// Memberwise Clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
