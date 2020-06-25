using System;
using System.Collections.Generic;
using code0k_cc.Standalone;
using code0k_cc.Parse;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime.ValueOfType
{
    sealed class FunctionDeclarationValue : IStandalone, IValueOfType
    {
        public bool IsConstant { get; set; } = true;

        /// <summary>
        /// Instance of CompoundStatement
        /// </summary>
        public ParseUnitInstance Instance;

        public string FunctionName;
        public NType ReturnType;
        /// <summary>
        /// VarName is from FunctionImplementation
        /// </summary>
        public List<(string VarName, NType Type)> Arguments;
        public BasicBlock ParentBlock;
        public UInt32 MaxLoop;

    }
}
