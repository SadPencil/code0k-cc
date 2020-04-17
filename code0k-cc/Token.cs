﻿using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class Token
    {
        public TokenType TokenType;
        public String Value;
        public long LocationInSourceCode;

        public override string ToString()
        {
            return base.ToString() +" "+ this.Value.ToString();
        }
    }
}
