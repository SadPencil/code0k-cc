using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class EnvironmentBlock
    {
        public EnvironmentBlock ReturnBlock;
        public EnvironmentBlock ParentBlock;
        public readonly Dictionary<string, RuntimeValue> Variables = new Dictionary<string, RuntimeValue>();
        public ParseInstance ParseInstance;

        public EnvironmentBlock LocateVariable(string name)
        {
            if (this.Variables.ContainsKey(name))
            {
                return this ;
            }
            else
            {
                if (this.ParentBlock == null)
                {
                    throw new Exception($"Unexpected variable \"{name}\".");
                }
                else
                {
                    return this.ParentBlock.LocateVariable(name);
                }
            }
        }
        public RuntimeValue GetVariableValue(string name)
        {
            if (this.Variables.ContainsKey(name))
            {
                return this.Variables[name];
            }
            else
            {
                if (this.ParentBlock == null)
                {
                    throw new Exception($"Unexpected variable \"{name}\".");
                }
                else
                {
                    return this.ParentBlock.GetVariableValue(name);
                }
            }
        }
    }
}
