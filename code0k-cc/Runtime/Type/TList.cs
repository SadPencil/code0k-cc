using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TList : IGenericsType
    {
        public override string TypeCodeName => base.TypeCodeName + "__List";

        private List<IType> list = new List<IType>();

        public override Dictionary<string, PropertyOperationDescription> PropertyDeclarations => new Dictionary<string, PropertyOperationDescription>()
        {
            {"Length", new PropertyOperationDescription()
            {
                PropertyName = "Length",
                ReturnType = TType.UInt32,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>()},
                Execute =(block,arg)  => new TUInt32() {Value = (UInt32)this.list.Count}
            }},
            {"Append", new PropertyOperationDescription()
            {
                PropertyName = "Append",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(this.T[0],"Item"),}},
                Execute = (block,arg)  =>
                {
                    //todo do assign (nizk)
                    this.list.Add(arg.Parameters.Parameters[0].Value.ImplicitConvertTo(this.T[0]).Assign());
                    return new TVoid();
                }
            }},
            {"Get", new PropertyOperationDescription()
            {
                PropertyName = "Get",
                ReturnType = this.T[0],
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(TType.UInt32, "Index"),}},
                Execute = (block,arg) =>
                {

                }
            }},

        };


        //todo property
    }
}
