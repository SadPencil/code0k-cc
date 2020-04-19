using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Parse;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime
{
    struct FunctionDeclarationValue
    {
        /// <summary>
        /// Instance of CompoundStatement
        /// </summary>
        public ParseInstance Instance;

        public string FunctionName;
        public NType ReturnType;
        /// <summary>
        /// VarName is from FunctionImplementation
        /// </summary>
        public List<(string VarName, NType Type)> Arguments;
        public EnvironmentBlock ParentBlock;
    }
}
