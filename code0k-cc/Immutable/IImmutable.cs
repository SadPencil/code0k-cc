using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Immutable
{
    abstract class IImmutable
    {
        protected Guid Guid { get; } = Guid.NewGuid();

        public override bool Equals(object obj)
        {
            if (obj is IImmutable o)
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

        public static bool operator ==(IImmutable op1, IImmutable op2)
        {
            return op1?.Guid == op2?.Guid;
        }
        public static bool operator !=(IImmutable op1, IImmutable op2)
        {
            return !( op1 == op2 );
        }
    }
}
