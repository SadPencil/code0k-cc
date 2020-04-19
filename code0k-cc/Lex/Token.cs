using System;

namespace code0k_cc.Lex
{
    class Token
    {
        public TokenType TokenType;
        public String Value;
        public int Row;
        public int Column;

        public override string ToString()
        {
            return base.ToString() +" "+ this.Value.ToString();
        }
    }
}
