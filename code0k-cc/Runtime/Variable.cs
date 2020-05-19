using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Pinocchio;
using code0k_cc.Standalone;
using code0k_cc.Runtime.ValueOfType;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Runtime
{
    class Variable : IStandalone
    {
        public NType Type => this.RawVariable.Type;
        public IValueOfType Value => this.RawVariable.Value;

        public readonly RawVariable RawVariable;

        public Variable(RawVariable rawVariable)
        {
            this.RawVariable = rawVariable;
        }

        /// <summary>
        /// Get all parent connections. Note that children connections are not saved.
        /// </summary>
        public readonly List<VariableConnection> ParentConnections = new List<VariableConnection>();
        // todo make RawVariable class with Type and Value, without ParentConnections
        // and make Variable class of RawVariable and ParentConnections

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

        public (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints) ToPinocchioWires(
            PinocchioCommonArg commonArg, bool checkRange)
        {
            return this.Type.ToPinocchioWires(this, commonArg, checkRange);
        }

    }
}
