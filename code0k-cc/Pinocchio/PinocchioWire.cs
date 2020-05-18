using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class PinocchioWire : IStandalone
    {
        // Value == null means that it is not an constant wire
        public readonly BigInteger? Value = null;

        public PinocchioWire(BigInteger? value)
        {
            this.Value = value;
        }
    }
}
