using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TUInt32 : IType
    {
        public override string TypeCodeName => "uint32";
        public override Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations =>
            new Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)>()
        {
            {UnaryOperation.UnaryPlus,( new UnaryOperationDescription() {
                Operation = UnaryOperation.UnaryPlus,
                Op1Type = TType.UInt32,
                RetType = TType.UInt32,
                }, () =>  new TUInt32(+this.Value) )},
            {UnaryOperation.BitwiseNot,( new UnaryOperationDescription() {
                Operation = UnaryOperation.BitwiseNot,
                Op1Type = TType.UInt32,
                RetType = TType.UInt32,
            }, () => new TUInt32( ~this.Value) )},
            {UnaryOperation.UnaryMinus,( new UnaryOperationDescription() {
                Operation = UnaryOperation.UnaryMinus,
                Op1Type = TType.UInt32,
                RetType = TType.UInt32,
            }, () => new TUInt32( 0 - this.Value) )},
        };

        public override Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)> BinaryOperations =>
            new Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)>()
        {
            {BinaryOperation.Addition, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Addition,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32(this.Value+((TUInt32)o).Value) )},
            {BinaryOperation.Subtract, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Subtract,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32(this.Value-((TUInt32)o).Value) )},
            {BinaryOperation.Multiplication, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Multiplication,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32(this.Value*((TUInt32)o).Value) )},
            {BinaryOperation.Division, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Division,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32( this.Value/((TUInt32)o).Value))},
            {BinaryOperation.Remainder, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Remainder,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32(this.Value%((TUInt32)o).Value) )},
            {BinaryOperation.BitwiseAnd, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.BitwiseAnd,
                        Op1Type = TType.UInt32,
                        Op2Type = TType.UInt32,
                        RetType = TType.UInt32,
                    }, (o) => new TUInt32(this.Value&((TUInt32)o).Value) )},
            {BinaryOperation.BitwiseOr, (new BinaryOperationDescription() {
                Operation = BinaryOperation.BitwiseOr,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.UInt32,
            }, (o) => new TUInt32(this.Value|((TUInt32)o).Value) )},
            {BinaryOperation.BitwiseXor, (new BinaryOperationDescription() {
                Operation = BinaryOperation.BitwiseOr,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.UInt32,
            }, (o) => new TUInt32(this.Value^((TUInt32)o).Value) )},

            {BinaryOperation.EqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.EqualTo,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value==((TUInt32)o).Value))},
            {BinaryOperation.NotEqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.NotEqualTo,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value!=((TUInt32)o).Value) )},
            {BinaryOperation.LessThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LessThan,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value<((TUInt32)o).Value) )},
            {BinaryOperation.GreaterThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.GreaterThan,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value>((TUInt32)o).Value) )},
            {BinaryOperation.LessEqualThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LessEqualThan,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value<=((TUInt32)o).Value) )},
            {BinaryOperation.GreaterEqualThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.GreaterEqualThan,
                Op1Type = TType.UInt32,
                Op2Type = TType.UInt32,
                RetType = TType.Bool,
            }, (o) => new TBool(this.Value>=((TUInt32)o).Value) )},
        };

        public readonly UInt32 Value;

        public TUInt32() { }

        public TUInt32(UInt32 value)
        {
            this.Value = value;
        }
    }
}
