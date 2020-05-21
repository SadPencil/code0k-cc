using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using code0k_cc.Config;
using code0k_cc.CustomException;
using code0k_cc.Pinocchio.Constraint;
using code0k_cc.Runtime;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.VariableMap;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class PinocchioArithmeticCircuit : IStandalone
    {
        private readonly VariableMap VariableMap;
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
                if (wireToID.ContainsKey(wire)) { return; }
                wireToID.Add(wire, wireList.Count);
                wireList.Add(wire);
            }

            var conList = new List<IPinocchioConstraint>();

            void AddConstraint(IPinocchioConstraint con)
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
                // 1
                {
                    commonArg.OneWire = new PinocchioWire();
                    AddWire(commonArg.OneWire);

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    con.ConstVariableWires = new PinocchioVariableWires(
                        NType.Field.GetCommonConstantValue(VariableCommonConstant.One).RawVariable,
                        commonArg.OneWire);
                }
                // 0
                {
                    commonArg.ZeroWire = new PinocchioWire();
                    AddWire(commonArg.ZeroWire);

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    con.ConstVariableWires = new PinocchioVariableWires(
                        NType.Field.GetCommonConstantValue(VariableCommonConstant.Zero).RawVariable,
                        commonArg.ZeroWire);
                }
                // -1
                {
                    commonArg.MinusOneWire = new PinocchioWire();
                    AddWire(commonArg.MinusOneWire);

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    con.ConstVariableWires = new PinocchioVariableWires(
                        NType.Field.GetCommonConstantValue(VariableCommonConstant.MinusOne).RawVariable,
                        commonArg.MinusOneWire);
                }
                // 2
                commonArg.PowerOfTwoWires = new PinocchioWire[My.Config.ModulusPrimeField_Prime_Bit + 1];
                commonArg.PowerOfTwoWires[0] = commonArg.OneWire;

                var powerOfTwoBaseVariable = NType.Field.GetCommonConstantValue(VariableCommonConstant.One);

                foreach (int i in Enumerable.Range(1, My.Config.ModulusPrimeField_Prime_Bit))
                {
                    commonArg.PowerOfTwoWires[i] = new PinocchioWire();
                    AddWire(commonArg.PowerOfTwoWires[i]);

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    powerOfTwoBaseVariable = NType.Field.BinaryOperation(
                        powerOfTwoBaseVariable,
                        powerOfTwoBaseVariable,
                        VariableOperationType.Binary_Addition);

                    con.ConstVariableWires = new PinocchioVariableWires(powerOfTwoBaseVariable.RawVariable,
                        commonArg.PowerOfTwoWires[i]);
                }
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
                                var con = new UserPrivateInputConstraint();
                                AddConstraint(con);
                                con.TypeWires = new PinocchioTypeWires(output.VariableWires);
                            }
                            else if (variableNode.NizkAttribute == NizkVariableType.Input)
                            {
                                output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                                var con = new UserInputConstraint();
                                AddConstraint(con);
                                con.TypeWires = new PinocchioTypeWires(output.VariableWires);
                            }
                            else
                            {
                                output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                                var con = new ConstWireConstraint();
                                AddConstraint(con);
                                con.ConstVariableWires = output.VariableWires;
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

            foreach (var constraint in conList)
            {
                switch (constraint)
                {
                    case BasicPinocchioConstraint basicPinocchioConstraint:
                        StringBuilder sb = new StringBuilder();
                        switch (basicPinocchioConstraint.Type)
                        {
                            case BasicPinocchioConstraintType.Add: _ = sb.Append("add"); break;
                            case BasicPinocchioConstraintType.Mul: _ = sb.Append("mul"); break;
                            case BasicPinocchioConstraintType.Or: _ = sb.Append("or"); break;
                            case BasicPinocchioConstraintType.Pack: _ = sb.Append("pack"); break;
                            case BasicPinocchioConstraintType.Split: _ = sb.Append("split"); break;
                            case BasicPinocchioConstraintType.Xor: _ = sb.Append("xor"); break;
                            case BasicPinocchioConstraintType.ZeroP: _ = sb.Append("zerop"); break;

                            default:
                                throw CommonException.AssertFailedException();
                        }

                       _= sb.Append(" in ");
                        _ = sb.Append(basicPinocchioConstraint.InWires.Count.ToString(CultureInfo.InvariantCulture));
                        _ = sb.Append(" <");
                        foreach (var wire in basicPinocchioConstraint.InWires)
                        {
                            _ = sb.Append(" " + wireToID[wire].ToString(CultureInfo.InvariantCulture));
                        }

                        _ = sb.Append(" > out ");
                        _ = sb.Append(basicPinocchioConstraint.OutWires.Count.ToString(CultureInfo.InvariantCulture));
                        _ = sb.Append(" <");
                        foreach (var wire in basicPinocchioConstraint.OutWires)
                        {
                            _ = sb.Append(" " + wireToID[wire].ToString(CultureInfo.InvariantCulture));
                        }
                        _ = sb.Append(" >");

                        outputWriter.WriteLine(sb.ToString());

                        break;
                    case UserInputConstraint userInputConstraint:
                        userInputConstraint.TypeWires.Wires.ForEach(wire => outputWriter.WriteLine("input " + wireToID[wire]));
                        break;
                    case UserPrivateInputConstraint userPrivateInputConstraint:
                        userPrivateInputConstraint.TypeWires.Wires.ForEach(wire => outputWriter.WriteLine("nizkinput " + wireToID[wire]));
                        break;
                    case ConstWireConstraint constWireConstraint:
                        constWireConstraint.ConstVariableWires.Wires.ForEach(wire => outputWriter.WriteLine("input " + wireToID[wire]));
                        break;
                    case DivModConstraint divModConstraint:
                        //todo
                        throw new NotImplementedException();

                    default:
                        throw CommonException.AssertFailedException();
                }
            }

            // todo: write the output wires at the end
        }




    }
}
