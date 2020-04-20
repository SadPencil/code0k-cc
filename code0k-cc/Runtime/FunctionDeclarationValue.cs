﻿using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Parse;  

namespace code0k_cc.Runtime
{
    class FunctionDeclarationValue
    {
        /// <summary>
        /// Instance of CompoundStatement
        /// </summary>
        public ParseInstance Instance;

        public string FunctionName;
        public NType ReturnType;
        /// <summary>
        /// VarName is from FunctionImplementation
        /// </summary>
        public List<(string VarName, NType Type)> Arguments;
        public EnvironmentBlock ParentBlock;
    }
}
