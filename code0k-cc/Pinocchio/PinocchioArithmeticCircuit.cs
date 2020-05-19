using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using code0k_cc.Config;
using code0k_cc.CustomException;
using code0k_cc.Runtime;
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

            var rawVarToWires = new Dictionary<RawVariable, PinocchioVariableWires>();

            void AddVariableWires(PinocchioVariableWires variableWires)
            {
                if (rawVarToWires.ContainsKey(variableWires.RawVariable)) return;

                rawVarToWires.Add(variableWires.RawVariable, variableWires);

                variableWires.Wires.ForEach(AddWire);
            }

            void AddPinocchioOutput(PinocchioOutput ret)
            {
                ret.AnonymousWires.ForEach(AddWire);
                AddVariableWires(ret.VariableWires);
                ret.Constraints.ForEach(AddConstraint);
            }


            var commonArg = new PinocchioCommonArg();

            {
                var OneWire = new PinocchioWire(null);
                AddWire(OneWire);
                commonArg.OneWire = OneWire;

                var ZeroWire = new PinocchioWire(null);
                AddWire(ZeroWire);
                commonArg.ZeroWire = ZeroWire;

                var MinusOneWire = new PinocchioWire(null);
                AddWire(MinusOneWire);
                commonArg.MinusOneWire = MinusOneWire;

                var ZeroConstWire = new PinocchioWire(BigInteger.Zero);
                AddWire(ZeroConstWire);

                var mulZeroConstraint = new PinocchioConstraint(PinocchioConstraintType.Mul);
                AddConstraint(mulZeroConstraint);
                mulZeroConstraint.InWires.Add(OneWire);
                mulZeroConstraint.InWires.Add(ZeroConstWire);
                mulZeroConstraint.OutWires.Add(ZeroWire);

                var MinusOneConstWire = new PinocchioWire(My.Config.ModulusPrimeField_Prime - 1);
                AddWire(MinusOneConstWire);

                var mulMinusOneConstraint = new PinocchioConstraint(PinocchioConstraintType.Mul);
                AddConstraint(mulMinusOneConstraint);
                mulMinusOneConstraint.InWires.Add(OneWire);
                mulMinusOneConstraint.InWires.Add(MinusOneConstWire);
                mulMinusOneConstraint.OutWires.Add(MinusOneWire);
            }

            foreach (var node in this.VariableMap.TopologicalSort())
            {

                switch (node)
                {
                    //todo
                    case VariableNode variableNode:
                        // a variable node is either an input/nizk/constant node, which is NOT produced by constraints
                        // or a non-constant intermediate/output node, which has already been produced by constraints
                        if (!rawVarToWires.ContainsKey(variableNode.RawVariable))
                        {
                            Debug.Assert(
                                ( variableNode.NizkAttribute == NizkVariableType.Intermediate && variableNode.RawVariable.Value.IsConstant ) ||
                                variableNode.NizkAttribute == NizkVariableType.Input ||
                                variableNode.NizkAttribute == NizkVariableType.NizkInput);

                            PinocchioOutput output;

                            // policy: checkRange is applied for nizkinput, and not applied for others
                            if (variableNode.NizkAttribute == NizkVariableType.NizkInput)
                            {
                                output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, true);
                            }
                            else
                            {
                                output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                            }

                            AddPinocchioOutput(output);
                        }
                        else
                        {
                            Debug.Assert(variableNode.NizkAttribute == NizkVariableType.Intermediate ||
                                         variableNode.NizkAttribute == NizkVariableType.Output);
                            Debug.Assert(!variableNode.RawVariable.Value.IsConstant);
                        }

                        break;
                    case OperationNode operationNode:
                        // get all in-variable
                        List<PinocchioVariableWires> inVars = new List<PinocchioVariableWires>();
                        foreach (var prevNode in operationNode.PrevNodes)
                        {
                            var varNode = (VariableNode) prevNode;
                            Debug.Assert(rawVarToWires.ContainsKey(varNode.RawVariable));

                            inVars.Add(rawVarToWires[varNode.RawVariable]);
                        }

                        // currently, assume at least one inVar
                        Debug.Assert(inVars.Count >= 1);

                        // for every outputVar, produce new PinocchioOutput according to the operation
                        foreach (var outNode in operationNode.NextNodes)
                        {
                            var outVarNode = (VariableNode) outNode;
                            PinocchioOutput output = inVars[0].RawVariable.Type.OperationNodeToPinocchio(
                                operationNode.ConnectionType,
                                inVars,
                                outVarNode.RawVariable,
                                commonArg
                            );

                            AddPinocchioOutput(output);
                        }
                        break;
                    default:
                        throw CommonException.AssertFailedException();
                }
            }

            // todo: write these wire & constraints

            // todo: write the output wires at the end
        }




    }
}
