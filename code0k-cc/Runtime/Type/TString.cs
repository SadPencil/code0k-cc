using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TString : IType
    {
        public override string TypeCodeName => "string";

        //todo support '==' and '+' and '!=' 

        public string Value = String.Empty;
    }
}
