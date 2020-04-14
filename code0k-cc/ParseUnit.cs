using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class ParseUnit
    {
        public ParseUnitType Type;
        public ParseUnitChildType ChildType;
        public List<ParseUnit> Children;
        public TokenType LeafNodeTokenType;
        public string Name;
    }
}
