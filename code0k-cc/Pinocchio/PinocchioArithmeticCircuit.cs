using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using code0k_cc.Runtime.VariableMap;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class PinocchioArithmeticCircuit : IStandalone
    {
        private VariableMap VariableMap;
        public PinocchioArithmeticCircuit(VariableMap variableMap)
        {
            this.VariableMap = variableMap;
        }

        public void OutputCircuit(TextWriter outputWriter)
        {
            var VariableNodeToWires = new Dictionary<VariableNode, List<PinocchioWire>>();

            var wireList = new List<PinocchioWire>();
            var wireToID = new Dictionary<PinocchioWire, int>();

            void AddWire(PinocchioWire wire)
            {
                if (wireToID.ContainsKey(wire)) { return; }
                wireToID.Add(wire, wireList.Count);
                wireList.Add(wire);
            }

            var OneWire = new PinocchioWire(null);

            var ZeroWire = new PinocchioWire(null);



            foreach (var node in this.VariableMap.TopologicalSort())
            {
                switch (node)
                {
                    //todo
                    case VariableNode variableNode:
                        break;
                    case VariableOperationNode operationNode:
                        break;

                    default:
                        throw new Exception("Assert failed!");
                }
            }

        }




    }
}
