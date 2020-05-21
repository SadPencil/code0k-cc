using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using code0k_cc.CustomException;
using code0k_cc.Runtime.Nizk;

namespace code0k_cc.Runtime.VariableMap
{
    class VariableMap
    {
        private HashSet<IVariableMapNode> nodes = new HashSet<IVariableMapNode>();
        public IReadOnlyCollection<IVariableMapNode> Nodes => this.nodes;

        private List<IVariableMapNode> rootNodes = new List<IVariableMapNode>();
        public IReadOnlyList<IVariableMapNode> RootNodes => this.rootNodes;

        private VariableMap() { }

        public static VariableMap GetMapFromVariableConnection(ICollection<VariableRef> inputVariables, ICollection<VariableRef> nizkVariables, ICollection<VariableRef> outputVariables)
        {
            var varToVarRefs = new Dictionary<RawVariable, VariableRef>();
            foreach (var inputVariable in inputVariables)
            {
                varToVarRefs.Add(inputVariable.Variable.RawVariable, inputVariable);
            }
            foreach (var nizkVariable in nizkVariables)
            {
                varToVarRefs.Add(nizkVariable.Variable.RawVariable, nizkVariable);
            }
            foreach (var outputVariable in outputVariables)
            {
                varToVarRefs.Add(outputVariable.Variable.RawVariable, outputVariable);
            }

            var varToNode = new Dictionary<RawVariable, VariableNode>();

            var conToNode = new Dictionary<VariableConnection, OperationNode>();

            var mapAllNodes = new List<IVariableMapNode>();
            var mapRootNodes = new List<IVariableMapNode>();

            VariableNode AddVariableNode(Variable itemVariable)
            {
                if (varToNode.ContainsKey(itemVariable.RawVariable))
                {
                    return varToNode[itemVariable.RawVariable];
                }

                VariableNode varNode;
                if (varToVarRefs.ContainsKey(itemVariable.RawVariable))
                {
                    var varRef = varToVarRefs[itemVariable.RawVariable];
                    switch (varRef.NizkAttribute)
                    {
                        case NizkVariableType.Input:
                        case NizkVariableType.NizkInput:
                        case NizkVariableType.Output:
                            varNode = new VariableNode(varRef);
                            break;

                        case NizkVariableType.Intermediate:
                        default:
                            throw CommonException.AssertFailedException();
                    }
                }
                else
                {
                    varNode = new VariableNode(itemVariable);
                }

                varToNode.Add(itemVariable.RawVariable, varNode);
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
                            var conNode = new OperationNode()
                            {
                                ConnectionType = connection.OperationType,
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

            foreach (var variableRef in outputVariables)
            {
                _ = AddVariableNode(variableRef.Variable);
            }

            Debug.Assert(mapAllNodes.Where(node => node.PrevNodes.Count == 0).Intersect(mapRootNodes).Count() == mapRootNodes.Count);




            foreach (var mapNode in mapRootNodes)
            {
                // only the following are permitted: (IsConstant=True) or (IsConstant=false and input/nizk)

                if (mapNode is VariableNode varNode)
                {
                    if (varNode.RawVariable.Value.IsConstant)
                    {
                        // okay
                    }
                    else
                    {
                        // is special var: has varRef
                        if (varToVarRefs.ContainsKey(varNode.RawVariable))
                        {
                            Debug.Assert(!varNode.RawVariable.Value.IsConstant);

                            var varRef = varToVarRefs[varNode.RawVariable];
                            switch (varRef.NizkAttribute)
                            {
                                case NizkVariableType.Input:
                                case NizkVariableType.NizkInput:
                                    break;

                                case NizkVariableType.Output:
                                    throw new Exception($"Runtime error: output variable \"{varRef.VarName}\" has not been assigned.");

                                case NizkVariableType.Intermediate:
                                default:
                                    throw CommonException.AssertFailedException();
                            }
                        }

                    }

                }
                else
                {
                    throw CommonException.AssertFailedException();
                }

            }

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
            var remainingOrphanNodes = new List<IVariableMapNode>(this.RootNodes);

            while (true)
            {
                Debug.Assert(remainingNodes.Where(node => nodePrevsDict[node].Count == 0).Intersect(remainingOrphanNodes).Count() == remainingOrphanNodes.Count);

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
                _ = remainingNodes.Remove(currentNode);

                // update children
                foreach (var node in currentNode.NextNodes)
                {
                    _ = nodePrevsDict[node].Remove(currentNode);
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
