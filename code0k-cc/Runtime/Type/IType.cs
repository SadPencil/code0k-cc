using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    interface IType
    {
        public abstract string TypeCodeName { get; }
        //public abstract IType Execute(EnvironmentBlock block, IType value, IRuntimeTypeExecuteArg arg);
        public abstract IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg);
        public abstract IType Assign(EnvironmentBlock block, TTypeOfType targetType, IRuntimeAssignArg arg);
        public abstract bool ToBool();
        public abstract Int32 ToInt32();
        public abstract Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations { get; }
        public abstract Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)> BinaryOperations { get; }

    }
}
