using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TDescriptionWords : IType
    {
        public string TypeCodeName => "__DescriptionWords";
        public IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }

        public Func<EnvironmentBlock, TTypeOfType, IRuntimeAssignArg, IType> Assign => null;
        public Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations => new Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)>();
        public Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)> BinaryOperations => new Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)>();


        public List<DescriptionWord> DescriptionWords;
    }
}
