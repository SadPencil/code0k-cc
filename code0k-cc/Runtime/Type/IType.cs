using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Runtime.AssignArg;
using code0k_cc.Runtime.ExecuteArg;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    abstract class IType
    {
        /// <summary>
        /// To determine whether two types are actually the same.
        /// </summary>
        public abstract string TypeCodeName { get; }
        
        /// <summary>
        /// Given target type <paramref name="type"/>, implicit convert to a new value of this type.
        /// Specially, if the type <paramref name="type"/> is exactly the same type, it can return either itself or a new value.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="type"></param>
        /// <param name="assignArg"></param>
        /// <returns></returns>
        public virtual IType Assign(EnvironmentBlock block, TType type, AssignExecuteArg assignArg) => this.ImplicitConvertTo(type); 
        
        protected virtual IType ImplicitConvertTo(TType targetType)
        {
            if (targetType.TypeCodeName == this.TypeCodeName)
            {
                return this;
            }
            else
            {
                //convert if possible & not losing any information
                throw new Exception($"Can not implicit convert to type \"{ this.TypeCodeName }\" from \"{targetType.TypeCodeName}\".");
            }
        }

        public virtual IType ExplicitConvertTo(TType targetType)
        {
            if (targetType.TypeCodeName == this.TypeCodeName)
            {
                return this;
            }
            else
            {
                //convert if possible
                throw new Exception($"Can not explicit convert to type \"{ this.TypeCodeName }\" from \"{targetType.TypeCodeName}\".");
            }
        }
        public virtual Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)> UnaryOperations { get; } = new Dictionary<UnaryOperation, (UnaryOperationDescription Description, Func<IType> OperationFunc)>();
        public virtual Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)> BinaryOperations { get; } = new Dictionary<BinaryOperation, (BinaryOperationDescription Description, Func<IType, IType> OperationFunc)>();
        public virtual Dictionary<string, TypeMethodDescription> InstanceMethodDeclarations { get; } = new Dictionary<string, TypeMethodDescription>();
        public virtual Dictionary<string, TypeMethodDescription> StaticMethodDeclarations { get; } = new Dictionary<string, TypeMethodDescription>();


    }
}
