using System.Collections.Generic;
using code0k_cc.Parse;
using code0k_cc.Runtime.Block;

namespace code0k_cc.Runtime.ValueOfType
{
    class FunctionDeclarationValue : IValueOfType
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
        public BasicBlock ParentBlock;
    }
}
