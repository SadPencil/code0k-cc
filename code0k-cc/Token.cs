using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
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
