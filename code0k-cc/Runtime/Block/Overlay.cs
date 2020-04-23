using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.Block
{
    class Overlay
    {
        public Guid Guid { get; } = Guid.NewGuid();

        public Overlay ParentOverlay { get; }

        public Overlay(Overlay parent)
        {
            this.ParentOverlay = parent;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Overlay overlay)
            {
                return this.Guid == overlay.Guid;
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

        public static bool operator ==(Overlay op1, Overlay op2)
        {
            return op1?.Guid == op2?.Guid;
        }
        public static bool operator !=(Overlay op1, Overlay op2)
        {
            return !( op1 == op2 );
        }
    }
}
