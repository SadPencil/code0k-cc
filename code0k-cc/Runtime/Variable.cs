using System;
using System.Collections.Generic;
using System.Text; 

namespace code0k_cc.Runtime
{
    class Variable
    {
        public NType Type;
        public object Value; //remember to clone the object
         
        public string String()
        {
            return this.Type.String(this);
        }

        public Variable Assign(NType leftType)
        {
            return leftType.Assign(this, leftType);
        }
    }
}
