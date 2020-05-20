using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Runtime
{
    class VariableOperationType : IStandalone
    {
        public string OperationCodeName { get; }
        public VariableOperationTypeType Type { get; }

        private VariableOperationType(string OperationCodeName, VariableOperationTypeType Type)
        {
            this.OperationCodeName = OperationCodeName;
            this.Type = Type;
        }

        public override string ToString()
        {
            return this.OperationCodeName;
        }

        public static readonly VariableOperationType TypeCast_Trim = new VariableOperationType("TypeCast_Trim", VariableOperationTypeType.TypeCast);
        public static readonly VariableOperationType TypeCast_NoCheckRange = new VariableOperationType("TypeCast_NoCheckRange", VariableOperationTypeType.TypeCast);

        public static readonly VariableOperationType Unary_Addition = new VariableOperationType("Unary_Addition", VariableOperationTypeType.Unary);
        public static readonly VariableOperationType Unary_Subtract = new VariableOperationType("Unary_Subtract", VariableOperationTypeType.Unary);
        public static readonly VariableOperationType Unary_BooleanNot = new VariableOperationType("Unary_BooleanNot", VariableOperationTypeType.Unary);
        public static readonly VariableOperationType Unary_BitwiseNot = new VariableOperationType("Unary_BitwiseNot", VariableOperationTypeType.Unary);

        public static readonly VariableOperationType Binary_Addition = new VariableOperationType("Binary_Addition", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_Subtract = new VariableOperationType("Binary_Subtract", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_Multiplication = new VariableOperationType("Binary_Multiplication", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_Division = new VariableOperationType("Binary_Division", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_Remainder = new VariableOperationType("Binary_Remainder", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BitwiseAnd = new VariableOperationType("Binary_BitwiseAnd", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BitwiseOr = new VariableOperationType("Binary_BitwiseOr", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BitwiseXor = new VariableOperationType("Binary_BitwiseXor", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BooleanAnd = new VariableOperationType("Binary_BooleanAnd", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BooleanOr = new VariableOperationType("Binary_BooleanOr", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_BooleanXor = new VariableOperationType("Binary_BooleanXor", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_EqualTo = new VariableOperationType("Binary_EqualTo", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_LessThan = new VariableOperationType("Binary_LessThan", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_LessEqualThan = new VariableOperationType("Binary_LessEqualThan", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_GreaterThan = new VariableOperationType("Binary_GreaterThan", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_GreaterEqualThan = new VariableOperationType("Binary_GreaterEqualThan", VariableOperationTypeType.Binary);
        public static readonly VariableOperationType Binary_NotEqualTo = new VariableOperationType("Binary_NotEqualTo", VariableOperationTypeType.Binary);

    }
}
