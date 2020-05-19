﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using code0k_cc.Runtime.Nizk;
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
            var variableNodeToWires = new Dictionary<VariableNode, List<PinocchioWire>>();

            var wireList = new List<PinocchioWire>();
            var wireToID = new Dictionary<PinocchioWire, int>();

            void AddWire(PinocchioWire wire)
            {
                // currently, any const-wire are not saved,
                // and these const-wires are handled at constraint.
                // this behavior will be changed later.
                if (wire.Value != null) { return; }


                if (wireToID.ContainsKey(wire)) { return; }
                wireToID.Add(wire, wireList.Count);
                wireList.Add(wire);
            }

            var conList = new List<PinocchioConstraint>();

            void AddConstraint(PinocchioConstraint con)
            {
                conList.Add(con);
            }

            var commonArg = new PinocchioCommonArg();

            {
                var OneWire = new PinocchioWire(null);
                AddWire(OneWire);
                commonArg.OneWire = OneWire;

                var ZeroWire = new PinocchioWire(null);
                AddWire(ZeroWire);
                commonArg.ZeroWire = ZeroWire;

                var ZeroConstWire = new PinocchioWire(BigInteger.Zero);
                AddWire(ZeroConstWire);

                var mulZeroConstraint = new PinocchioConstraint(PinocchioConstraintType.Mul);
                AddConstraint(mulZeroConstraint);
                mulZeroConstraint.InWires.Add(OneWire);
                mulZeroConstraint.InWires.Add(ZeroConstWire);
                mulZeroConstraint.OutWires.Add(ZeroWire);
            }

            foreach (var node in this.VariableMap.TopologicalSort())
            {

                switch (node)
                {
                    //todo
                    case VariableNode variableNode:
                        // a variable node is either an input/nizk/constant node, which is NOT produced by constraints
                        // or a non-constant intermediate/output node, which has already been produced by constraints
                        if (!variableNodeToWires.ContainsKey(variableNode))
                        {
                            Debug.Assert(
                                (variableNode.NizkAttribute == NizkVariableType.Intermediate &&
                                 variableNode.Variable.Value.IsConstant) ||
                                variableNode.NizkAttribute == NizkVariableType.Input ||
                                variableNode.NizkAttribute == NizkVariableType.NizkInput);

                            (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints) ret;

                            // policy: checkRange is applied for nizkinput, and not applied for others
                            if (variableNode.NizkAttribute == NizkVariableType.NizkInput)
                            {
                                ret = variableNode.Variable.ToPinocchioWires(commonArg, true);
                            }
                            else
                            {
                                ret = variableNode.Variable.ToPinocchioWires(commonArg, false);
                            }

                            ret.Wires.ForEach(AddWire);
                            ret.Constraints.ForEach(AddConstraint);

                            variableNodeToWires.Add(variableNode, ret.Wires);
                        }
                        else
                        {
                            Debug.Assert(variableNode.NizkAttribute == NizkVariableType.Intermediate ||
                                         variableNode.NizkAttribute == NizkVariableType.Output);
                            Debug.Assert(!variableNode.Variable.Value.IsConstant);
                        }

                        break;
                    case VariableOperationNode operationNode:
                        // get all in-variable
                        foreach (var prevNode in operationNode.PrevNodes)
                        {
                            if (prevNode is VariableNode varNode)
                            {
                                Debug.Assert(variableNodeToWires.ContainsKey(varNode));
                            }
                            else
                            {
                                throw new Exception("Assert failed!");
                            }
                        }
                        // todo: produce new out wire & constraints,
                        // make connection between each out variable and the corresponding List<wire>
                        // and save them to the board by ret.Wires.ForEach(AddWire); ret.Constraints.ForEach(AddConstraint);


                        ret = op
                        break;

                    default:
                        throw new Exception("Assert failed!");
                } 
            }

            // todo: write these wire & constraints

            // todo: write the output wires at the end
        }




    }
}
