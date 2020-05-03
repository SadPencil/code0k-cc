using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.ExeArg
{
    class CallStack
    {
        public FunctionDeclarationValue FunctionDeclaration { get; }
        public CallStack ParentCallStack { get; }

        public CallStack(FunctionDeclarationValue function, CallStack parent)
        {
            this.FunctionDeclaration = function;
            this.ParentCallStack = parent;
        }

        public int GetFunctionCount() => GetFunctionCount(this.FunctionDeclaration);
        public int GetFunctionCount(FunctionDeclarationValue function) =>
            ( ( function == this.FunctionDeclaration ) ? 1 : 0 ) + ( this.ParentCallStack?.GetFunctionCount(function) ?? 0 );
    }
}
