using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Pinocchio
{
    class PinocchioOutput
    {
        public PinocchioVariableWires VariableWires;
        public readonly List<IPinocchioConstraint> Constraints = new List<IPinocchioConstraint>();
        public readonly List<PinocchioWire> AnonymousWires = new List<PinocchioWire>();
    }
}
