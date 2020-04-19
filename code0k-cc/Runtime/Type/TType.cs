using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace code0k_cc.Runtime.Type
{
    /// <summary>
    /// The type of an IGenericsType
    /// </summary>
    sealed class TType : IType
    {
        public override string TypeCodeName => "__Type__Generics__" + this.TypeTypeCodeName;

        private System.Type SystemType;
        private System.Type[] ConstructorArgTypes;
        private object[] ConstructorParameters;
        private string TypeTypeCodeName; 

        public IType GetInstance()
        { 
            var instance = this.SystemType?.GetConstructor(this.ConstructorArgTypes)?.Invoke(this.ConstructorParameters);
            return (IType) instance;
        }

        /// <summary>
        /// Initialize the type of an IType <paramref name="T"/>.
        /// </summary> 
        private TType(System.Type type, System.Type[] constructorArgTypes, object[] constructorParameters)  
        {
            if (!( type.IsSubclassOf(typeof(IType)) ))
            {
                throw new Exception("Assert failed! 'System.Type type' should be a sub class of IType");
            }

            this.SystemType = type;
            this.ConstructorArgTypes = constructorArgTypes;
            this.ConstructorParameters = constructorParameters;

            this.TypeTypeCodeName = this.GetInstance().TypeCodeName;
        }

        public static readonly TType Bool = new TType(typeof(TBool), System.Type.EmptyTypes, Array.Empty<object>());
        public static readonly TType UInt32 = new TType(typeof(TUInt32), System.Type.EmptyTypes, Array.Empty<object>());
        public static readonly TType Void = new TType(typeof(TVoid), System.Type.EmptyTypes, Array.Empty<object>());




    }
}
