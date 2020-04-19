using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TBool : IType
    {
        public override string TypeCodeName => "bool";

        public override Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations =>
           new Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)>() {
                {UnaryOperation.LogicalNot, (new UnaryOperationDescription() {
            Operation = UnaryOperation.LogicalNot,
                                Op1Type = TType.Bool,
                                RetType = TType.Bool,
                        }, ()=>new TBool( !this.Value) )},

                {UnaryOperation.BitwiseNot, (new UnaryOperationDescription() {
             Operation = UnaryOperation.BitwiseNot,
                    Op1Type = TType.Bool,
                    RetType = TType.Bool,
                }, ()=>new TBool( !this.Value) ) },
           };


        public override Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)> BinaryOperations =>
            new Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)>() {
            {BinaryOperation.LogicalAnd, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalAnd,
                Op1Type = TType.Bool,
                Op2Type = TType.Bool,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value && ((TBool)o).Value) )},
            {BinaryOperation.LogicalXor, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalXor,
                Op1Type = TType.Bool,
                Op2Type = TType.Bool,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value != ((TBool)o).Value) )},
            {BinaryOperation.LogicalOr, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalOr,
                Op1Type = TType.Bool,
                Op2Type = TType.Bool,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value || ((TBool)o).Value) )},

            {BinaryOperation.EqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.EqualTo,
                Op1Type = TType.Bool,
                Op2Type = TType.Bool,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value==((TBool)o).Value) )},
            {BinaryOperation.NotEqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.NotEqualTo,
                Op1Type = TType.Bool,
                Op2Type = TType.Bool,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value!=((TBool)o).Value) )},

        };

        public readonly bool Value;

        public TBool() { }

        public TBool(bool value)
        {
            this.Value = value;
        }

    }
}
