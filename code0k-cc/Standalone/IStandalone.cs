using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Standalone
{
    abstract class IStandalone
    {
        protected Guid Guid { get; } = Guid.NewGuid();

        public override bool Equals(object obj)
        {
            if (obj is IStandalone o)
            {
                return this.Guid == o.Guid;
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

        public static bool operator ==(IStandalone op1, IStandalone op2)
        {
            return op1?.Guid == op2?.Guid;
        }
        public static bool operator !=(IStandalone op1, IStandalone op2)
        {
            return !( op1 == op2 );
        }
    }
}
