using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class PinocchioConstraint : IStandalone
    {
        public readonly PinocchioConstraintType Type;
        // note that these list are ordered
        public readonly List<PinocchioWire> InWires = new List<PinocchioWire>();
        public readonly List<PinocchioWire> OutWires = new List<PinocchioWire>();

        public PinocchioConstraint(PinocchioConstraintType type)
        {
            this.Type = type;
        }
    }
}
