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
                                OP1Type = new TTypeOfType(this),
                                RetType = new TTypeOfType(this),
                        }, ()=>new TBool() { Value = !this.Value }) },

                {UnaryOperation.BitwiseNot, (new UnaryOperationDescription() {
        Operation = UnaryOperation.BitwiseNot,
                    OP1Type = new TTypeOfType(this),
                    RetType = new TTypeOfType(this),
                }, ()=>new TBool() { Value = !this.Value }) },
           };


        public override Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)> BinaryOperations =>
            new Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)>() {
            {BinaryOperation.LogicalAnd, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalAnd,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value && ((TBool)o).Value})},
            {BinaryOperation.LogicalXor, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalXor,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value != ((TBool)o).Value})},
            {BinaryOperation.LogicalOr, (new BinaryOperationDescription() {
                Operation = BinaryOperation.LogicalOr,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value || ((TBool)o).Value})},

            {BinaryOperation.EqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.EqualTo,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value==((TBool)o).Value})},
            {BinaryOperation.NotEqualTo, (new BinaryOperationDescription() {
                Operation = BinaryOperation.NotEqualTo,
                OP1Type = new TTypeOfType(this),
                OP2Type = new TTypeOfType(this),
                RetType = new TTypeOfType(new TBool()),
            }, (o) => new TBool(){Value=this.Value!=((TBool)o).Value})},

        };

        public bool Value;
    }
}
