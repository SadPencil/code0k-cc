using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Runtime.VariableMap
{
    abstract class IVariableMapNode : IStandalone
    {
        //Note that these two list are ORDERED 
        public List<IVariableMapNode> NextNodes { get; } = new List<IVariableMapNode>();
        public List<IVariableMapNode> PrevNodes { get; } = new List<IVariableMapNode>();
    }
}
