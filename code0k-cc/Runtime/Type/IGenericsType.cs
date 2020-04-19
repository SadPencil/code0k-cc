using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace code0k_cc.Runtime.Type
{
    abstract class IGenericsType : IType
    {
        public override string TypeCodeName { get; }
        
        public IReadOnlyList<TType> T { get; }
        
        public IGenericsType() => throw new NotSupportedException();

        public IGenericsType(IReadOnlyList<TType> T) : base()
        {
            this.T = T;
            var name = "__GenericsType";
            foreach (var t in T)
            {
                name += t.TypeCodeName;
            }

            this.TypeCodeName = name;
        }
    }
}
