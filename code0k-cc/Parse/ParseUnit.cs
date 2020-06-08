using System;
using System.Collections.Generic;
using code0k_cc.Lex;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExeArg;
using code0k_cc.Runtime.ExeResult;

namespace code0k_cc.Parse
{
    class ParseUnit
    {
        public ParseUnitType Type;
        public ParseUnitChildType ChildType;
        public IReadOnlyList<ParseUnit> Children;
        public TokenType TerminalTokenType;
        public string Name;

        public Func<ParseUnitInstance, ExeArg, ExeResult> Execute;
    }
}
