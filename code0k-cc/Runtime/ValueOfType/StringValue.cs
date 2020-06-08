using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.ValueOfType
{
    sealed class StringValue : IValueOfType
    {
        public bool IsConstant { get; set; } = true;
        public String Value;
    }
}
