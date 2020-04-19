using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TFunctionDeclaration : IType
    {
        public override string TypeCodeName => "__Function";

        public ParseInstance Instance;
        public string FunctionName;
        public TTypeOfType ReturnType;
        public TFunctionDeclarationArguments Arguments;

        public override IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg)
        {
            // call the function

            // prepare new environment
            EnvironmentBlock newBlock = new EnvironmentBlock()
            {
                ParentBlock = block.LocateVariable(this.FunctionName),
                ParseInstance = this.Instance,
                ReturnBlock = block,
            };

            // load function arguments if available
            if (arg != null)
            {
                var funcArg = (FunctionExecuteArg) arg;
                if (this.Arguments.Arguments.Count != funcArg.Parameters.Parameters.Count)
                {
                    throw new Exception($"Unexpected function arguments of function \"{this.FunctionName}\".");
                }

                foreach (var i in Enumerable.Range(0, funcArg.Parameters.Parameters.Count))
                {
                    var (value, argVarName) = funcArg.Parameters.Parameters[i];

                    // implicit convert
                    var newValue = value.ImplicitConvertTo(this.Arguments.Arguments[i].Type);

                    newBlock.Variables.Add(argVarName, newValue);

                }
            }

            if (this.Instance == null)
            {
                throw new Exception($"Unimplemented function \"{this.FunctionName}\"");
            }

            return this.Instance.Execute(newBlock, null);
        }



    }
}
