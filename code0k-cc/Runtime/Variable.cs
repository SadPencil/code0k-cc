using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Pinocchio;
using code0k_cc.Standalone;
using code0k_cc.Runtime.ValueOfType;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Runtime
{
    class Variable
    {
        public NType Type => this.RawVariable.Type;
        public IValueOfType Value => this.RawVariable.Value;

        //todo: [MaxPossibleValue]

        public readonly RawVariable RawVariable;

        public Variable(RawVariable rawVariable)
        {
            this.RawVariable = rawVariable;
        }

        /// <summary>
        /// Get all parent connections. Note that children connections are not saved.
        /// </summary>
        public readonly List<VariableConnection> ParentConnections = new List<VariableConnection>();
        public string GetString()
        {
            return this.Type.GetVariableString(this.RawVariable);
        }

        public Variable Assign(NType leftType)
        {
            return this.Type.Assign(this, leftType);
        }

        public Variable ExplicitConvert(NType leftType)
        {
            return this.Type.ExplicitConvert(this, leftType);
        }
        public Variable InternalConvert(NType leftType)
        {
            return this.Type.InternalConvert(this, leftType);
        }

    }
}
