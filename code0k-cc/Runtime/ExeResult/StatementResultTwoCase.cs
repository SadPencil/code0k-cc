﻿using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.ExeResult
{
    class StatementResultTwoCase : StatementResult
    {
        public Variable Expression;
        public StatementResult TrueCase;
        public StatementResult FalseCase;
    }
}