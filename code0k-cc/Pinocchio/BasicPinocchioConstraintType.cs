using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using code0k_cc.Runtime;
using code0k_cc.Runtime.VariableMap;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    enum BasicPinocchioConstraintType
    {
        Mul,
        Add,
        //MulConst, 
        //MulConstNeg, 
        ZeroP,
        Xor,
        Or,
        Split,
        Pack,
    }
}
