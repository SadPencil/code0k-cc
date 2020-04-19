using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using code0k_cc.Parse;

namespace code0k_cc.Runtime
{
    static class Runtime
    {
        public ExeResult.ExeResult CallFunction(ParseInstance instance, ExeArg.ExeArg.ExeArg arg)
        {
            // call the function

            // prepare new environment
            EnvironmentBlock newBlock = new EnvironmentBlock()
            {
                ParentBlock = arg.Block.LocateVariableBlock(arg.FuncExeArg..FunctionName),
                ParseInstance = this.Instance,
                ReturnBlock = block,
            };

            // load function arguments if available
            if (assignArg != null)
            {
                if (this.Arguments.Arguments.Count != funcArg.Parameters.Parameters.Count)
                {
                    throw new Exception($"Unexpected function arguments of function \"{this.FunctionName}\".");
                }

                foreach (var i in Enumerable.Range(0, funcArg.Parameters.Parameters.Count))
                {
                    //todo byval or byref?

                    // currently, byref...
                    // therefore, no implicit convert are allowed
                    var refParam = funcArg.Parameters.Parameters[i];

                    // maybe some special treatment for numbers?

                    var newValue = value.Assign(block, this.Arguments.Arguments[i].Type, assignArg);

                    newBlock.Variables.Add(argVarName, newValue);
                }
            }

            if (this.Instance == null)
            {
                throw new Exception($"Unimplemented function \"{this.FunctionName}\"");
            }

            return this.Instance.Execute(newBlock, null, assignArg);

        }
    }
}
