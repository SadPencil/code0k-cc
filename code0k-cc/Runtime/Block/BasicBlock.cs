using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.Block
{
    class BasicBlock
    {
        private Guid Guid { get; } = Guid.NewGuid();

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
            return "BasicBlock "+this.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj is BasicBlock blk)
            {
                return this.Guid == blk.Guid;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Guid);
        }

        public static bool operator ==(BasicBlock op1, BasicBlock op2)
        {
            return op1?.Guid == op2?.Guid;
        }
        public static bool operator !=(BasicBlock op1, BasicBlock op2)
        {
            return !( op1 == op2 );
        }
    }
}
