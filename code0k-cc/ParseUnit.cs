using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Type;

namespace code0k_cc
{
    class ParseUnit
    {
        public ParseUnitType Type;
        public ParseUnitChildType ChildType;
        public IReadOnlyList<ParseUnit> Children;
        public TokenType TerminalTokenType;
        public string Name;

        public Func<ParseInstance, EnvironmentBlock, FunctionExecuteArg, IType> Execute;
    }
}
