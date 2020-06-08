using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.Block;

namespace code0k_cc.Runtime.ExeResult
{
    class StatementResultOneCase : StatementResult
    {
        //todo make these fields readonly
        public Overlay Overlay;
        public StatementResultType ExecutionResultType;
        public Variable ReturnVariable ;
    }
}
