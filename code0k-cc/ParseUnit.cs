using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class ParseUnit
    {
        public ParseUnitType Type;
        public ParseUnitChildType ChildType;
        public IReadOnlyList<ParseUnit> Children;
        public TokenType TerminalTokenType;
        public string Name;
    }
}
