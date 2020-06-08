using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Pinocchio.Constraint;

namespace code0k_cc.Pinocchio
{
    class PinocchioSubOutput
    {
        public PinocchioVariableWires VariableWires;
        // note that these constraints are ordered.
        public readonly List<IPinocchioConstraint> Constraints = new List<IPinocchioConstraint>();
        public readonly List<PinocchioWire> AnonymousWires = new List<PinocchioWire>();
    }
}
