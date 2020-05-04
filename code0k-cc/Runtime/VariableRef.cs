using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Nizk;

namespace code0k_cc.Runtime
{
    class VariableRef : ICloneable
    {
        public Variable Variable;

        public NizkVariableType NizkAttribute = NizkVariableType.NonNizkVariable;

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
