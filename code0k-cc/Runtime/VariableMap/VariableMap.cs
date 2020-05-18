using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace code0k_cc.Runtime.VariableMap
{
    class VariableMap
    {
        private HashSet<IVariableMapNode> nodes = new HashSet<IVariableMapNode>();
        public IReadOnlyCollection<IVariableMapNode> Nodes => this.nodes;

        private List<IVariableMapNode> rootNodes = new List<IVariableMapNode>();
        public IReadOnlyList<IVariableMapNode> RootNodes => this.rootNodes;

        public VariableMap() { }

        public void AddNode(IVariableMapNode node)
        {
            if (this.nodes.Contains(node))
            {
                return;
            }

            _ = this.nodes.Add(node);
             
            // assert it's a root node. Otherwise, it should be already added in this.Nodes before
            Debug.Assert(!this.nodes.Any(mapNode => mapNode.NextNodes.Contains(mapNode)));

            this.rootNodes.Add(node);
            this.nodes.Add(node);

            node.NextNodes.ForEach(this.AddNode);
        }

    }
}
