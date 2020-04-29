using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.ExeResult
{
    class StatementResultTwoCase : StatementResult
    {
        //todo make these fields readonly
        public Variable Condition;
        public StatementResult TrueCase;
        public StatementResult FalseCase;
    }
}
