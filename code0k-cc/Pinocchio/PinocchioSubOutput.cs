using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Pinocchio.Constraint;

namespace code0k_cc.Pinocchio
{
    class PinocchioSubOutput
    {
        // These wires always comes first.
        public PinocchioVariableWires VariableWires;
        // Constraints are ordered. Topological orders must be satisfied.
        public readonly List<IPinocchioConstraint> Constraints = new List<IPinocchioConstraint>();
        // AnonymousWires are ordered (after VariableWires). Topological orders must be satisfied.
        public readonly List<PinocchioWire> AnonymousWires = new List<PinocchioWire>();
    }
}
