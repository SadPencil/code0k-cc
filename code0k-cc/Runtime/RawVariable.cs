using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ValueOfType;
using code0k_cc.Standalone;

namespace code0k_cc.Runtime
{
    class RawVariable : IStandalone
    {
        public NType Type;
        public IValueOfType Value;

        public RawVariable() { }

    }
}
