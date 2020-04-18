using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TString : IType
    {
        public string TypeCodeName => "String";
        public IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }


        public Func<EnvironmentBlock, TTypeOfType, IRuntimeAssignArg, IType> Assign => (block, typeOfType, arg) =>
        {
            if (typeOfType.IsTypeEquals(this))
            {
                return this;
            }
            else
            {
                throw new Exception($"Unexpected type when assigning variable." + Environment.NewLine +
                                    $"Supposed to be \"{ this.TypeCodeName }\", got \"{typeOfType.TypeCodeName}\" here.");
            }
        };
        public Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations => new Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)>();
        public Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)> BinaryOperations => new Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)>(
        //todo support '==' and '+' and '!='
        // maybe as well as < <= > >= ?
        );


        public string Value = String.Empty;
    }
}
