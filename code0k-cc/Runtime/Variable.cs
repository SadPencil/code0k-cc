using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;
using code0k_cc.Runtime.ValueOfType;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Runtime
{
    class Variable : IStandalone
    {
        public NType Type;
        public IValueOfType Value;

        /// <summary>
        /// Get all parent connections. Note that children connections are not saved.
        /// </summary>
        public readonly List<VariableConnection> ParentConnections = new List<VariableConnection>();

        public string GetString()
        {
            return this.Type.GetString(this);
        }

        public Variable Assign(NType leftType)
        {
            return this.Type.Assign(this, leftType);
        }

        public Variable ExplicitConvert(NType leftType)
        {
            return this.Type.ExplicitConvert(this, leftType);
        }

    }
}
