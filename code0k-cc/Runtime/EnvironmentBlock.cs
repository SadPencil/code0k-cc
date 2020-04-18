using System;
using System.Collections.Generic;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Runtime
{
    class EnvironmentBlock
    {
        public EnvironmentBlock ReturnBlock;
        public EnvironmentBlock ParentBlock;
        public readonly Dictionary<string, IType> Variables = new Dictionary<string, IType>();
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
        public IType GetVariableValue(string name)
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
