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

        private readonly List<IType> Value = new List<IType>();

        //todo add new()

        public override Dictionary<string, TypeMethodDescription> InstanceMethodDeclarations { get; }
        public override Dictionary<string, TypeMethodDescription> StaticMethodDeclarations { get; }


        public TList(IReadOnlyList<TType> T) : base(T)
        {
            if (T.Count != 1)
            {
                throw new Exception($"Type \"{this.TypeCodeName} \" only accept a single generics type.");
            }

            this.StaticMethodDeclarations = new Dictionary<string, TypeMethodDescription>()
            {
                {
                    "New",new TypeMethodDescription()
                    {
                        PropertyName = "New",
                        ReturnType = TType.GenericsType(typeof(TList),this.T),
                        Arguments = new TFunctionDeclarationArguments(){Arguments = new List<(TType Type, string VarName)>()},
                        Execute = (block, funcArg, assignArg) => new TList(this.T)
                    }
                },
            };


            this.InstanceMethodDeclarations = new Dictionary<string, TypeMethodDescription>() {
            {"Length", new TypeMethodDescription()
            {
                PropertyName = "Length",
                ReturnType = TType.UInt32,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>()},
                Execute = (block,funcArg,assignArg) => new TUInt32((UInt32)this.Value.Count)
            }},

            {"Clear", new TypeMethodDescription()
            {
                PropertyName = "Clear",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>()},
                Execute = (block, funcArg, assignArg) =>
                {
                    this.Value.Clear();
                    return new TVoid();
                }
            }},

            {"Append", new TypeMethodDescription()
            {
                PropertyName = "Append",
                ReturnType = TType.Void,
                Arguments = new TFunctionDeclarationArguments() {Arguments =  new List<(TType Type, string VarName)>() {(this.T[0],"Item"),}},
                Execute = (block,funcArg,assignArg)  =>
                {
                    this.Value.Add(funcArg.Parameters.Parameters[0].Value.Assign(block,this.T[0],assignArg));
                    return new TVoid();
                }
            }},

            {"Get", new TypeMethodDescription()
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

                    return this.Value[(Int32) index];
                }
            }},

            {"Set", new TypeMethodDescription()
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

                     this.Value[(Int32) index] = funcArg.Parameters.Parameters[1].Value.Assign(block,this.T[0],assignArg);
                     return new TVoid();
                },
            }},

            {"RemoteAt", new TypeMethodDescription()
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

                    this.Value.RemoveAt((Int32) index);
                    return new TVoid();
                }
            }},


        };

        }
    }
}
