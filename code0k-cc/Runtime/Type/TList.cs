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
                Execute = (block,funcArg,assignArg) => new TUInt32((UInt32)this.list.Count)
            }},

            {"Clear", new PropertyOperationDescription()
            {
                PropertyName = "Clear",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>()},
                Execute = (block, funcArg, assignArg) =>
                {
                    this.list.Clear();
                    return new TVoid();
                }
            }},

            {"Append", new PropertyOperationDescription()
            {
                PropertyName = "Append",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(this.T[0],"Item"),}},
                Execute = (block,funcArg,assignArg)  =>
                {
                    this.list.Add(funcArg.Parameters.Parameters[0].Value.Assign(block,this.T[0],assignArg));
                    return new TVoid();
                }
            }},

            {"Get", new PropertyOperationDescription()
            {
                PropertyName = "Get",
                ReturnType = this.T[0],
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(TType.UInt32, "Index"),}},
                Execute =  (block,funcArg,assignArg) =>
                {
                    UInt32 index = ((TUInt32) funcArg.Parameters.Parameters[0].Value).Value;
                    if (index > Int32.MaxValue)
                    {
                        throw new Exception($"The list can't hold more than {Int32.MinValue} items.");
                    }

                    return this.list[(Int32) index];
                }
            }},

            {"Set", new PropertyOperationDescription()
            {
                PropertyName = "Set",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(TType.UInt32, "Index"),(this.T[0], "Item")}},
                Execute =  (block,funcArg,assignArg) =>
                {
                    UInt32 index = ((TUInt32) funcArg.Parameters.Parameters[0].Value).Value;
                    if (index > Int32.MaxValue)
                    {
                        throw new Exception($"The list can't hold more than {Int32.MinValue} items.");
                    }

                     this.list[(Int32) index] = funcArg.Parameters.Parameters[1].Value.Assign(block,this.T[0],assignArg);
                     return new TVoid();
                },
            }},

            {"RemoteAt", new PropertyOperationDescription()
            {
                PropertyName = "RemoteAt",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(TType.UInt32, "Index"),}},
                Execute =  (block,funcArg,assignArg) =>
                {
                    UInt32 index = ((TUInt32) funcArg.Parameters.Parameters[0].Value).Value;
                    if (index > Int32.MaxValue)
                    {
                        throw new Exception($"The list can't hold more than {Int32.MinValue} items.");
                    }

                    this.list.RemoveAt((Int32) index);
                    return new TVoid();
                }
            }},


        };


    }
}
