using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime;

namespace code0k_cc.Pinocchio
{
    class PinocchioVariableWires
    { 
        public readonly RawVariable RawVariable;

        public readonly List<PinocchioWire> Wires = new List<PinocchioWire>();

        public PinocchioVariableWires(RawVariable rawVariable)
        {
            this.RawVariable = rawVariable;
        }

        public PinocchioVariableWires(RawVariable rawVariable, PinocchioWire singleWire) : this(rawVariable)
        {
            this.Wires.Add(singleWire);
        }

        public PinocchioVariableWires(RawVariable rawVariable, List<PinocchioWire> wires) : this(rawVariable)
        {
            wires.ForEach(this.Wires.Add);
        }
    }
}
