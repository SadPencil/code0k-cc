using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Runtime.Block
{
    class BasicBlock : IStandalone
    {
        //todo remove invalid overlay to save memory
        private Dictionary<Overlay, Dictionary<string, VariableRef>> Variables { get; } = new Dictionary<Overlay, Dictionary<string, VariableRef>>();

        public Dictionary<string, VariableRef> GetVariableDict(Overlay overlay)
        {
            if (this.Variables.ContainsKey(overlay))
            {
                return this.Variables[overlay];
            }
            else
            {
                this.Variables.Add(overlay, new Dictionary<string, VariableRef>());
                return this.Variables[overlay];
            }
        }

        public BasicBlock ParentBlock { get; set; }

        public BasicBlock(BasicBlock parent)
        {
            this.ParentBlock = parent;
        }

        public override string ToString()
        {
            return "BasicBlock " + this.Guid;
        }

    }
}
