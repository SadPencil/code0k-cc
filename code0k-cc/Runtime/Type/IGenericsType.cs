using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace code0k_cc.Runtime.Type
{
    abstract class IGenericsType : IType
    {
        private string _TypeCodeName = "__GenericsType";
        public override string TypeCodeName => this._TypeCodeName;

        private IReadOnlyList<TType> _T;

        public IReadOnlyList<TType> T {
            get => this._T;
            set {
                this._T = value;
                this._TypeCodeName = "__GenericsType";
                foreach (var t in value)
                {
                    this._TypeCodeName += t.TypeCodeName;
                }
            }
        }

        public IGenericsType() { }

        public IGenericsType(IReadOnlyList<TType> T)
        {
            this.T = T;
        }
    }
}
