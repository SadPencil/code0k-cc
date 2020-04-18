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
                OP1Type = new TTypeOfType(this),
                RetType = new TTypeOfType(this),
                }, () =>  new TUInt32(){Value = this.Value} )},
            {UnaryOperation.BitwiseNot,( new UnaryOperationDescription() {
                Operation = UnaryOperation.BitwiseNot,
                OP1Type = new TTypeOfType(this),
                RetType = new TTypeOfType(this),
            }, () => new TUInt32(){Value = ~this.Value} )},
            {UnaryOperation.UnaryMinus,( new UnaryOperationDescription() {
                Operation = UnaryOperation.UnaryMinus,
                OP1Type = new TTypeOfType(this),
                RetType = new TTypeOfType(this),
            }, () => new TUInt32(){Value = 0 - this.Value} )},
        };

        public override Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)> BinaryOperations =>
            new Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)>()
        {
            {BinaryOperation.Addition, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Addition,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value+((TUInt32)o).Value})},
            {BinaryOperation.Subtract, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Subtract,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value-((TUInt32)o).Value})},
            {BinaryOperation.Multiplication, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Multiplication,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value*((TUInt32)o).Value})},
            {BinaryOperation.Division, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Division,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value/((TUInt32)o).Value})},
            {BinaryOperation.Remainder, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.Remainder,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value%((TUInt32)o).Value})},
            {BinaryOperation.BitwiseAnd, (new BinaryOperationDescription() {
                        Operation = BinaryOperation.BitwiseAnd,
                        OP1Type = new TTypeOfType(this),
                        OP2Type = new TTypeOfType(this),
                        RetType = new TTypeOfType(this),
                    }, (o) => new TUInt32(){Value=this.Value&((TUInt32)o).Value})},
            {BinaryOperation.BitwiseOr, (new BinaryOperationDescription() {
                Operation = BinaryOperation.BitwiseOr,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(this),
            }, (o) => new TUInt32(){Value=this.Value|((TUInt32)o).Value})},
            {BinaryOperation.BitwiseXor, (new BinaryOperationDescription() {
                Operation = BinaryOperation.BitwiseOr,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(this),
            }, (o) => new TUInt32(){Value=this.Value^((TUInt32)o).Value})},

            {BinaryOperation.EqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.EqualTo,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value==((TUInt32)o).Value})},
            {BinaryOperation.NotEqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.NotEqualTo,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value!=((TUInt32)o).Value})},
            {BinaryOperation.LessThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LessThan,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value<((TUInt32)o).Value})},
            {BinaryOperation.GreaterThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.GreaterThan,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value>((TUInt32)o).Value})},
            {BinaryOperation.LessEqualThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LessEqualThan,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value<=((TUInt32)o).Value})},
            {BinaryOperation.GreaterEqualThan, (new BinaryOperationDescription() {
                Operation = BinaryOperation.GreaterEqualThan,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value>=((TUInt32)o).Value})},
        };

        public UInt32 Value;
    }
}
