using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.Block
{
    class BasicBlock
    {
        public Guid Guid { get; } = Guid.NewGuid();

        public Dictionary<Overlay, Dictionary<string, VariableRef>> Variables { get; } = new Dictionary<Overlay, Dictionary<string, VariableRef>>();
         
        public BasicBlock ParentBlock { get; set; }

        public BasicBlock(BasicBlock parent)
        {
            this.ParentBlock = parent;
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
