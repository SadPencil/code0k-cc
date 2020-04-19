using System;
using System.Collections.Generic;
using code0k_cc.Parse;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime
{
    class EnvironmentBlock
    {
        public EnvironmentBlock ReturnBlock;
        public EnvironmentBlock ParentBlock;
        private readonly Dictionary<string, VariableRef> Variables = new Dictionary<string, VariableRef>();
        public ParseInstance ParseInstance;

        public void AddVariable(string name, Variable value)
        {
            if (this.Variables.ContainsKey(name))
            {
                throw new Exception($"Variable \"{name}\" has already been declared at this scope.");
            }
            else
            {
                this.Variables.Add(name, new VariableRef(value));
            }
        } 

        public VariableRef GetVariableRef(string name, bool recursively)
        {
            EnvironmentBlock block;
            if (recursively)
            {
                block = LocateVariableBlock(name);
                return block.GetVariableRef(name, false);
            }
            else
            {
                block = this;
                if (this.Variables.ContainsKey(name))
                {
                    return this.Variables[name];
                }
                else
                {
                    throw new Exception($"Unexpected variable \"{name}\".");
                }
            }
        }

        public EnvironmentBlock LocateVariableBlock(string name)
        {
            if (this.Variables.ContainsKey(name))
            {
                return this;
            }
            else
            {
                if (this.ParentBlock == null)
                {
                    throw new Exception($"Unexpected variable \"{name}\".");
                }
                else
                {
                    return this.ParentBlock.LocateVariableBlock(name);
                }
            }
        }
    }
}
