﻿using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class ConstWireConstraint : IStandalone, IAdvancedPinocchioConstraint
    {
        public PinocchioVariableWires ConstVariableWires;
    }
}