using System;
using System.Collections.Generic;
using System.Linq;

namespace code0k_cc.Runtime.VariableMap
{
    static class VariableMapUtil
    {
        public static List<VariableNode> GetVariableReverses(ICollection<Variable> outputVariables)
        {
            var varToNode = new Dictionary<Variable, VariableNode>();
            var nodeToVar = new Dictionary<VariableNode, Variable>();

            var conToNode = new Dictionary<VariableConnection, VariableOperationNode>();
            var nodeToCon = new Dictionary<VariableOperationNode, VariableConnection>();

            var rootNodes = new List<VariableNode>();

            VariableNode AddVariableNode(Variable itemVariable)
            {
                if (varToNode.ContainsKey(itemVariable))
                {
                    return varToNode[itemVariable];
                }

                var varNode = new VariableNode(itemVariable);
                varToNode.Add(itemVariable, varNode);
                nodeToVar.Add(varNode, itemVariable);

                if (0 == ( itemVariable.ParentConnections?.Count ?? 0 ))
                {
                    rootNodes.Add(varNode);
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
                            nodeToCon.Add(conNode, connection);

                            //find other out var
                            foreach (var connectionOutVariable in connection.OutVariables)
                            {
                                var outNode = AddVariableNode(connectionOutVariable);
                                conNode.NextNodes.Add(outNode);
                                outNode.PrevNodes.Add(conNode);
                            }

                            //find other in var
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

            return rootNodes;
        }
    }
}
