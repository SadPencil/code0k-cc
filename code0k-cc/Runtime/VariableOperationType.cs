using System;
using System.Collections.Generic;
using System.Text; 
using code0k_cc.Standalone;

namespace code0k_cc.Runtime
{
    class VariableOperationType : IStandalone
    {
        public string OperationCodeName { get; }

        private VariableOperationType(string OperationCodeName)
        {
            this.OperationCodeName = OperationCodeName;
        }

        public static readonly VariableOperationType TypeCast = new VariableOperationType("TypeCast");

        public static readonly VariableOperationType Unary_Addition = new VariableOperationType("Unary_Addition");
        public static readonly VariableOperationType Unary_Subtract = new VariableOperationType("Unary_Subtract");
        public static readonly VariableOperationType Unary_BooleanNot = new VariableOperationType("Unary_BooleanNot");
        public static readonly VariableOperationType Unary_BitwiseNot = new VariableOperationType("Unary_BitwiseNot");
        
        public static readonly VariableOperationType Binary_Addition = new VariableOperationType("Binary_Addition");
        public static readonly VariableOperationType Binary_Subtract = new VariableOperationType("Binary_Subtract");
        public static readonly VariableOperationType Binary_Multiplication = new VariableOperationType("Binary_Multiplication");
        public static readonly VariableOperationType Binary_Division = new VariableOperationType("Binary_Division");
        public static readonly VariableOperationType Binary_Remainder = new VariableOperationType("Binary_Remainder");
        public static readonly VariableOperationType Binary_BitwiseAnd = new VariableOperationType("Binary_BitwiseAnd");
        public static readonly VariableOperationType Binary_BitwiseOr = new VariableOperationType("Binary_BitwiseOr");
        public static readonly VariableOperationType Binary_BitwiseXor = new VariableOperationType("Binary_BitwiseXor");
        public static readonly VariableOperationType Binary_BooleanAnd = new VariableOperationType("Binary_BooleanAnd");
        public static readonly VariableOperationType Binary_BooleanOr = new VariableOperationType("Binary_BooleanOr");
        public static readonly VariableOperationType Binary_BooleanXor = new VariableOperationType("Binary_BooleanXor");
        public static readonly VariableOperationType Binary_EqualTo = new VariableOperationType("Binary_EqualTo");
        public static readonly VariableOperationType Binary_LessThan = new VariableOperationType("Binary_LessThan");
        public static readonly VariableOperationType Binary_LessEqualThan = new VariableOperationType("Binary_LessEqualThan");
        public static readonly VariableOperationType Binary_GreaterThan = new VariableOperationType("Binary_GreaterThan");
        public static readonly VariableOperationType Binary_GreaterEqualThan = new VariableOperationType("Binary_GreaterEqualThan");
        public static readonly VariableOperationType Binary_NotEqualTo = new VariableOperationType("Binary_NotEqualTo");
        
    }
}
