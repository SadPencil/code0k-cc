using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Pinocchio
{
    class PinocchioTypeWires
    {
        public NType Type;
        public readonly List<PinocchioWire> Wires = new List<PinocchioWire>();

        public PinocchioTypeWires() { }

        public PinocchioTypeWires(NType type, PinocchioWire singleWire)
        {
            this.Type = type;
            this.Wires.Add(singleWire);
        }
        public PinocchioTypeWires(PinocchioVariableWires variableWires)
        {
            this.Type = variableWires.RawVariable.Type;
            variableWires.Wires.ForEach(this.Wires.Add);
        }
    }
}
