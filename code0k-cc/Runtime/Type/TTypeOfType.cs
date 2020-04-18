using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TTypeOfType : IType
    {
        public string TypeCodeName => "__Type";
        public IType Execute(EnvironmentBlock block, IRuntimeExecuteArg arg) { throw new Exception($"Type \"{this.TypeCodeName} \" can't be executed."); }
        public bool ToBool() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Bool\"."); }
        public int ToInt32() { throw new Exception($"Can't convert \"{this.TypeCodeName} \" to \"Int32\"."); }
        public Func<EnvironmentBlock, TTypeOfType, IRuntimeAssignArg, IType> Assign => null;
        public Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations => new Dictionary<TUnaryOperation, (BinaryOperationDescription Description, Func<IType> OperationFunc)>();
        public Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)> BinaryOperations => new Dictionary<TBinaryOperation, (UnaryOperation Description, Func<IType, IType> OperationFunc)>();

        public TTypeOfType()
        {
            // to support nested TypeOfType
            throw new NotImplementedException();
        }

        public TTypeOfType(string codeName)
        {
            //todo some magic here
            throw new NotImplementedException();
        }

        public TTypeOfType(IType value)
        {
            this.Type = value.GetType();
        }

        //todo support Generics and nested types

        private readonly System.Type Type;

        public IType GetInstance()
        {
            //todo assert this.Type is IType

            ConstructorInfo info = this.Type.GetConstructor(System.Type.EmptyTypes);
            if (info == null)
            {
                throw new Exception("Assert failed! There supposed to be a constructor with no arguments.");
            }
            return (IType) info.Invoke(Array.Empty<object>());
        }
        public String GetTypeCodeName() => this.GetInstance().TypeCodeName;

        public bool IsTypeEquals(IType value)
        {
            var thisTypeName = this.GetTypeCodeName();
            var thatTypeName = value.TypeCodeName;

            return thisTypeName == thatTypeName;
        }

        public bool IsImplicitConvertible(IType value)
        {
            //todo 
            return this.IsTypeEquals(value);
        }
    }
}
