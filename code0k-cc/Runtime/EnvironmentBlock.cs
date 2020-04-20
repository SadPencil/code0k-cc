using System;
using System.Collections.Generic;
using code0k_cc.Parse; 

namespace code0k_cc.Runtime
{
    class EnvironmentBlock
    {
        public EnvironmentBlock ReturnBlock;
        public EnvironmentBlock ParentBlock;
        private readonly Dictionary<string, VariableRef> Variables = new Dictionary<string, VariableRef>();
        public ParseInstance ParseInstance;

        /// <summary>
        /// Re-assign a variable to the given <paramref name="value"/>. The type of <paramref name="value"/> MUST be same with the existing item.
        /// If there is no existing item, the action is same as AddVariable.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void ReAssignVariable(string name, Variable value)
        {
            if (this.Variables.ContainsKey(name))
            {
                // check the type
                if (value.Type != this.Variables[name].Variable.Type)
                {
                    throw new Exception($"Type mismatch while assigning variable \"{name}\". " +
                                        $"Expected \"{ this.Variables[name].Variable.Type.TypeCodeName}\", got \"{value.Type.TypeCodeName}\".");
                }

                // only replace the variable, not the reference
                this.Variables[name].Variable = value;
            }
            else
            {
                this.AddVariable(name, value);
            }
        }

        public void AddVariable(string name, Variable value)
        {
            if (this.Variables.ContainsKey(name))
            {
                throw new Exception($"Variable \"{name}\" has already been declared at this scope.");
            }
            else
            {
                this.Variables.Add(name, new VariableRef(){Variable = value});
            }
        }

        public VariableRefRef GetVariableRefRef(string name, bool recursively)
        {
            if (recursively)
            {
                var block = this.LocateVariableBlock(name);
                return block.GetVariableRefRef(name, false);
            }
            else
            {
                if (this.Variables.ContainsKey(name))
                {
                    return new VariableRefRef(this.Variables[name]);
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
