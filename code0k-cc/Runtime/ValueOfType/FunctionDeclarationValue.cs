using System;
using System.Collections.Generic;
using code0k_cc.Immutable;
using code0k_cc.Parse;
using code0k_cc.Runtime.Block;

namespace code0k_cc.Runtime.ValueOfType
{
    sealed class FunctionDeclarationValue : IImmutable, IValueOfType
    {
        public bool IsConstant { get; set; } = true;

        private Guid Guid { get; } = Guid.NewGuid();

        /// <summary>
        /// Instance of CompoundStatement
        /// </summary>
        public ParseUnitInstance Instance;

        public string FunctionName;
        public NType ReturnType;
        /// <summary>
        /// VarName is from FunctionImplementation
        /// </summary>
        public List<(string VarName, NType Type)> Arguments;
        public BasicBlock ParentBlock;
        public UInt32 MaxLoop;


        public override bool Equals(object obj)
        {
            if (obj is FunctionDeclarationValue v)
            {
                return this.Guid == v.Guid;
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

        public static bool operator ==(FunctionDeclarationValue op1, FunctionDeclarationValue op2)
        {
            return op1?.Guid == op2?.Guid;
        }
        public static bool operator !=(FunctionDeclarationValue op1, FunctionDeclarationValue op2)
        {
            return !( op1 == op2 );
        }

    }
}
