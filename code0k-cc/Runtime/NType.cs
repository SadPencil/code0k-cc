using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace code0k_cc.Runtime
{
    class NType
    {
        /// <summary>
        /// The one and only one field to determine whether two types are actually the same.
        /// </summary>
        public string TypeCodeName { get; }
        /// <summary>
        /// Method to get a new value. Throw exceptions.
        /// </summary>
        public Func<Variable> NewValue { get; }
        /// <summary>
        /// Method to get a new value from a token string. Throw exceptions.
        /// </summary>
        public Func<string, Variable> Parse { get; }

        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        public Func<Variable, string> String { get; }

        /// <summary>
        /// Generics Type Lists. Can be null.
        /// </summary>
        public IReadOnlyList<NType> T { get; }

        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        public Func<Variable, NType,  Variable> Assign { get; }
        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        private Func<Variable, NType, Variable> ImplicitConvert { get; }

        private NType() { }

        public static readonly NType UInt32 = new NType()
        {
            //todo
        };



        public override bool Equals(object obj)
        {
            if (obj is NType ntype)
            {
                return this.TypeCodeName == ntype.TypeCodeName;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return this.TypeCodeName.GetHashCode();
        }

        public static bool operator ==(NType op1, NType op2)
        {
            return op1?.TypeCodeName == op2?.TypeCodeName;
        }
        public static bool operator !=(NType op1, NType op2)
        {
            return !( op1 == op2 );
        }

    }
}
