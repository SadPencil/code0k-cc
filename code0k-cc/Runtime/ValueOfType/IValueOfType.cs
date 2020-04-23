using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.ValueOfType
{
    abstract class IValueOfType
    {
        public bool IsConstant { get; set; }

        public IValueOfType()
        {
            this.IsConstant = true;
        }
    }
}
