using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Pinocchio
{
    class PinocchioOutput
    {
        public PinocchioVariableWires VariableWires;
        public readonly List<PinocchioConstraint> Constraints = new List<PinocchioConstraint>();
        public readonly List<PinocchioWire> AnonymousWires = new List<PinocchioWire>();
    }
}
