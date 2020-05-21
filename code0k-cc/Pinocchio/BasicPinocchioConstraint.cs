using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class BasicPinocchioConstraint : IStandalone, IPinocchioConstraint
    {
        public readonly BasicPinocchioConstraintType Type;
        // note that these list are ordered
        public readonly List<PinocchioWire> InWires = new List<PinocchioWire>();
        public readonly List<PinocchioWire> OutWires = new List<PinocchioWire>();

        public BasicPinocchioConstraint(BasicPinocchioConstraintType type)
        {
            this.Type = type;
        }
    }
}
