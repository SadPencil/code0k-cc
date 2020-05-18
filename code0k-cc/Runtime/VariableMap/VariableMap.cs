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

        private VariableMap() { }

        public static VariableMap GetMapFromVariableConnection(ICollection<Variable> outputVariables)
        {
            var varToNode = new Dictionary<Variable, VariableNode>();

            var conToNode = new Dictionary<VariableConnection, VariableOperationNode>();

            var mapAllNodes = new List<IVariableMapNode>();
            var mapRootNodes = new List<IVariableMapNode>();

            VariableNode AddVariableNode(Variable itemVariable)
            {
                if (varToNode.ContainsKey(itemVariable))
                {
                    return varToNode[itemVariable];
                }

                var varNode = new VariableNode(itemVariable);
                varToNode.Add(itemVariable, varNode);
                mapAllNodes.Add(varNode);

                if (0 == ( itemVariable.ParentConnections?.Count ?? 0 ))
                {
                    mapRootNodes.Add(varNode);
                }
                else
                {
                    foreach (var connection in itemVariable.ParentConnections)
                    {
                        //save connection 
                        if (!conToNode.ContainsKey(connection))
                        {
                            var conNode = new VariableOperationNode()
                            {
                                ConnectionType = connection.Type,
                            };

                            conToNode.Add(connection, conNode);
                            mapAllNodes.Add(conNode);

                            //find other out var
                            //note: keep the order of conNode.NextNodes
                            foreach (var connectionOutVariable in connection.OutVariables)
                            {
                                var outNode = AddVariableNode(connectionOutVariable);
                                conNode.NextNodes.Add(outNode);
                                outNode.PrevNodes.Add(conNode);
                            }

                            //find other in var
                            //note: keep the order of conNode.PrevNodes
                            foreach (var connectionInVariable in connection.InVariables)
                            {
                                var inNode = AddVariableNode(connectionInVariable);
                                conNode.PrevNodes.Add(inNode);
                                inNode.NextNodes.Add(conNode);
                            }

                        }
                    }
                }

                return varNode;
            }

            foreach (var variable in outputVariables)
            {
                _ = AddVariableNode(variable);
            }

            Debug.Assert(mapAllNodes.Where(node => node.PrevNodes.Count == 0).SequenceEqual(mapRootNodes));

            return new VariableMap()
            {
                nodes = mapAllNodes.ToHashSet(),
                rootNodes = mapRootNodes,
            };
        }

        public IEnumerable<IVariableMapNode> TopologicalSort()
        {
            // copy prev nodes list
            var nodePrevsDict = new Dictionary<IVariableMapNode, ISet<IVariableMapNode>>();
            foreach (var variableMapNode in this.Nodes)
            {
                nodePrevsDict.Add(variableMapNode, new HashSet<IVariableMapNode>(variableMapNode.PrevNodes));
            }

            // copy node collection
            var remainingNodes = new HashSet<IVariableMapNode>(this.Nodes);
            var remainingOrphanNodes = new List<IVariableMapNode>(this.RootNodes); // remainingNodes.Where(node => nodePrevsDict[node].Count == 0);

            while (true)
            {
                Debug.Assert(remainingNodes.Where(node => nodePrevsDict[node].Count == 0).SequenceEqual(remainingOrphanNodes));

                //find orphan node
                if (remainingOrphanNodes.Count <= 0)
                {
                    //assert there are no ring
                    if (remainingNodes.Count == 0)
                    {
                        yield break;
                    }
                    else
                    {
                        throw new Exception("Ring detected. Can't do topological sorting.");
                    }
                }

                var currentNode = remainingOrphanNodes[0];
                remainingOrphanNodes.RemoveAt(0);

                // remove it
                remainingNodes.Remove(currentNode);

                // update children
                foreach (var node in currentNode.NextNodes)
                {
                    nodePrevsDict[node].Remove(currentNode);
                    if (nodePrevsDict[node].Count == 0)
                    {
                        remainingOrphanNodes.Add(node);
                    }
                }

                yield return currentNode;
            }
        }


        //public void AddNode(IVariableMapNode node)
        //{
        //    if (this.nodes.Contains(node))
        //    {
        //        return;
        //    }

        //    _ = this.nodes.Add(node);

        //    // assert it's a root node. Otherwise, it should be already added in this.Nodes before
        //    Debug.Assert(!this.nodes.Any(mapNode => mapNode.NextNodes.Contains(mapNode)));

        //    this.rootNodes.Add(node);
        //    this.nodes.Add(node);

        //    node.NextNodes.ForEach(this.AddNode);
        //}

    }
}
