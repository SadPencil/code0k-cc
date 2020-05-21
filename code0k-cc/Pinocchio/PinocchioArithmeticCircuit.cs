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
using code0k_cc.Runtime.ValueOfType;
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
            var wireToID = new Dictionary<PinocchioWire, int>();

            void AddWire(PinocchioWire wire)
            {
                if (wireToID.ContainsKey(wire)) { return; }
                wireToID.Add(wire, wireToID.Keys.Count);
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

                    var comment = new CommentConstraint();
                    AddConstraint(comment);
                    comment.Comment = "The constant wire, value 1";

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

                    var comment = new CommentConstraint();
                    AddConstraint(comment);
                    comment.Comment = "The constant wire, value 0";

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

                    var comment = new CommentConstraint();
                    AddConstraint(comment);
                    comment.Comment = "The constant wire, value -1";

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

                    powerOfTwoBaseVariable = NType.Field.BinaryOperation(
                        powerOfTwoBaseVariable,
                        powerOfTwoBaseVariable,
                        VariableOperationType.Binary_Addition);

                    var comment = new CommentConstraint();
                    AddConstraint(comment);
                    comment.Comment = "The constant wire, value " + ( (NizkFieldValue) powerOfTwoBaseVariable.RawVariable.Value ).Value.ToString(CultureInfo.InvariantCulture);

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    con.ConstVariableWires = new PinocchioVariableWires(powerOfTwoBaseVariable.RawVariable,
                        commonArg.PowerOfTwoWires[i]);
                }
            }

            var outputVariables = new HashSet<RawVariable>();

            foreach (var node in this.VariableMap.TopologicalSort())
            {

                switch (node)
                {
                    //todo mul outputnode by 1?
                    case VariableNode variableNode:
                        // a variable node is either an input/nizk/constant node, which is NOT produced by constraints
                        // or a non-constant intermediate/output node, which has already been produced by constraints

                        if (variableNode.NizkAttribute == NizkVariableType.Output)
                        {
                            if (!outputVariables.Contains(variableNode.RawVariable))
                            {
                                _ = outputVariables.Add(variableNode.RawVariable);
                            }
                        }

                        if (!rawVarToWires.ContainsKey(variableNode.RawVariable))
                        {
                            Debug.Assert(
                                ( ( variableNode.NizkAttribute == NizkVariableType.Intermediate || variableNode.NizkAttribute == NizkVariableType.Output ) && variableNode.RawVariable.Value.IsConstant ) ||
                                variableNode.NizkAttribute == NizkVariableType.Input ||
                                variableNode.NizkAttribute == NizkVariableType.NizkInput);

                            PinocchioOutput output;
                            if (variableNode.RawVariable.Value.IsConstant)
                            {
                                output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                                var comment = new CommentConstraint();
                                AddConstraint(comment);
                                comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wire(s) is a constant value.";

                                var con = new ConstWireConstraint();
                                AddConstraint(con);
                                con.ConstVariableWires = output.VariableWires;
                            }
                            else
                            {
                                switch (variableNode.NizkAttribute)
                                {
                                    // policy: checkRange is applied for nizkinput, and not applied for others
                                    case NizkVariableType.NizkInput:
                                        {
                                            output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, true);

                                            var comment = new CommentConstraint();
                                            AddConstraint(comment);
                                            comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wire(s) are nizk-input wire of variable \"{variableNode.VarName}\".";

                                            var con = new UserPrivateInputConstraint();
                                            AddConstraint(con);
                                            con.TypeWires = new PinocchioTypeWires(output.VariableWires);
                                            break;
                                        }
                                    case NizkVariableType.Input:
                                        {
                                            output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);

                                            var comment = new CommentConstraint();
                                            AddConstraint(comment);
                                            comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wire(s) are input wire of variable \"{variableNode.VarName}\".";

                                            var con = new UserInputConstraint();
                                            AddConstraint(con);
                                            con.TypeWires = new PinocchioTypeWires(output.VariableWires);
                                            break;
                                        }
                                    default:
                                        throw CommonException.AssertFailedException();
                                }
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

            // todo: optimize redundancy wire & constraints            
            // note: even if all the output of some constraints are not used, it is NOT considered useless
            // example here: input 1; split in 1 <1> out 32 <2,3,4,...,33>; this constraint ensure that "input 1" is smaller than 2^32, and must not be deleted while optimizing

            // write these wire & constraints
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

                        _ = sb.Append(" in ");
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
                    case CommentConstraint commentConstraint:
                        outputWriter.WriteLine("//" + commentConstraint.Comment);
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
