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

        private readonly System.Type SystemType;
        private readonly System.Type[] ConstructorArgTypes;
        private readonly object[] ConstructorParameters;
        private readonly string TypeTypeCodeName;

        public IType GetInstance()
        {
            // todo what if get instance of given value?
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

        public static readonly TType Bool = NonGenericsType(typeof(TBool));
        public static readonly TType UInt32 = NonGenericsType(typeof(TUInt32));
        public static readonly TType Void = NonGenericsType(typeof(TVoid));
        public static readonly TType String = NonGenericsType(typeof(TString));

        public static TType GenericsType(System.Type type, IReadOnlyList<TType> T)
        {
            if (!( type.IsSubclassOf(typeof(IGenericsType)) ))
            {
                throw new Exception("Assert failed! 'System.Type type' should be a sub class of IGenericsType");
            }

            return new TType(type, new System.Type[] { typeof(IReadOnlyList<TType>) }, new object[] { T });
        }

        public static TType NonGenericsType(System.Type type)
        {
            if (( type.IsSubclassOf(typeof(IGenericsType)) ))
            {
                throw new Exception("Assert failed! 'System.Type type' should not be a sub class of IGenericsType");
            }

            return new TType(type, System.Type.EmptyTypes, Array.Empty<object>());
        }



    }
}
