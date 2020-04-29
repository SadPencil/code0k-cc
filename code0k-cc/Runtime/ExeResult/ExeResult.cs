﻿using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.ExeResult
{
    class ExeResult
    {
        // union; one of them has value, others == null
        public TypeResult TypeUnitResult;
        public GenericsTypeResult GenericsTypeResult;
        public ExpressionResult ExpressionResult; 
        public FunctionDeclarationValue FunctionDeclarationValue;
        public StatementResult StatementResult;
    }
}
