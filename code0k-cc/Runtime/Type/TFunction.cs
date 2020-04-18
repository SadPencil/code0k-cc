using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.ExecuteArg;

namespace code0k_cc.Runtime.Type
{
    class TFunction : IType
    {
        public string TypeCodeName => "__Function";
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }

        public ParseInstance Instance;
        public string FunctionName;
        public TTypeOfType ReturnType;
        public TFunctionDeclarationArguments Arguments;

        public IType Execute(EnvironmentBlock block, IRuntimeTypeExecuteArg arg)
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
                if (this.Arguments.Arguments.Count != funcArg.Arguments.Arguments.Count)
                {
                    throw new Exception($"Unexpected function arguments of function \"{this.FunctionName}\".");
                }

                foreach (var i in Enumerable.Range(0, funcArg.Arguments.Arguments.Count))
                {
                    var (value, argVarName) = funcArg.Arguments.Arguments[i];
                    
                    if (!this.Arguments.Arguments[i].Type.IsImplicitConvertible(value))
                    {
                        throw new Exception($"Unexpected function argument \"{argVarName}\" of function \"{this.FunctionName}\"." + Environment.NewLine +
                        $"Supposed to be \"{ this.Arguments.Arguments[i].Type.GetTypeCodeName() }\", got \"{value.TypeCodeName}\" here.");
                    }

                    //todo do Implicit Convert 

                    newBlock.Variables.Add(argVarName, value);

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
