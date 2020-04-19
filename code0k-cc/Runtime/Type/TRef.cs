using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using code0k_cc.Runtime.Operation;

namespace code0k_cc.Runtime.Type
{
    class TRef : IGenericsType
    {
        public override string TypeCodeName => base.TypeCodeName + "__Ref";
        private readonly EnvironmentBlock EnvironmentBlock;
        private readonly string VariableName;

        public override Dictionary<string, TypeMethodDescription> InstanceMethodDeclarations { get; }
        public override Dictionary<string, TypeMethodDescription> StaticMethodDeclarations { get; }

        public TRef(IReadOnlyList<TType> T, EnvironmentBlock block, string varName) : this(T)
        {
            // must be the exactly block
            // in case of same-name var
            block = block.LocateVariable(varName);

            Debug.Assert(block.Variables.ContainsKey(varName));

            this.EnvironmentBlock = block;
            this.VariableName = varName;
        }
        public TRef(IReadOnlyList<TType> T) : base(T)
        {
            if (T.Count != 1)
            {
                throw new Exception($"Type \"{this.TypeCodeName} \" only accept a single generics type.");
            }

            this.StaticMethodDeclarations = new Dictionary<string, TypeMethodDescription>()
            {
                {"New", new TypeMethodDescription() {
                        PropertyName = "New",
                        ReturnType = TType.GenericsType(typeof(TRef), this.T),
                        Arguments = new TFunctionDeclarationArguments(){Arguments = new List<(TType Type, string VarName)>(){(this.T[0],"Variable")}},
                        Execute = (block, funcArg, assignArg) => new TRef(this.T,this.EnvironmentBlock, ((TString) funcArg.Parameters.Parameters[0].Value).Value)
                    }},
            };

            this.InstanceMethodDeclarations = new Dictionary<string, TypeMethodDescription>()
            {
                { "Value",new TypeMethodDescription() {
                    PropertyName = "Value",//de-reference
                    ReturnType =  this.T[0],
                    Arguments =  new TFunctionDeclarationArguments(){Arguments = new List<(TType Type, string VarName)>()},
                    Execute = (block, funcArg, assignArg) => this.EnvironmentBlock.Variables[this.VariableName]
                }},
            };

        }
    }
}
