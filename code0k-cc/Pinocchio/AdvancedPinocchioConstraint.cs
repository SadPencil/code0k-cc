using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class AdvancedPinocchioConstraint : IStandalone, IPinocchioConstraint
    {
        public readonly AdvancedPinocchioConstraint Type;
        // note that these list are ordered
        public readonly List<PinocchioTypeWires> InWires = new List<PinocchioTypeWires>();
        public readonly List<PinocchioTypeWires> OutWires = new List<PinocchioTypeWires>();

        public AdvancedPinocchioConstraint(AdvancedPinocchioConstraint type)
        {
            this.Type = type;
        }
    }
}
