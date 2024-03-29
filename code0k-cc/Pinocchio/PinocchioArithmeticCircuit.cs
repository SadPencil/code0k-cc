﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using code0k_cc.Config;
using code0k_cc.CustomException;
using code0k_cc.Pinocchio.Constraint;
using code0k_cc.Runtime;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.Type;
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

        public void OutputCircuit(TextWriter arithWriter, TextWriter inHelperWriter)
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
                if (rawVarToWires.ContainsKey(variableWires.RawVariable)) { return; }

                rawVarToWires.Add(variableWires.RawVariable, variableWires);

                variableWires.Wires.ForEach(AddWire);
            }

            void AddPinocchioSubOutput(PinocchioSubOutput ret)
            {
                AddVariableWires(ret.VariableWires);

                ret.AnonymousWires.ForEach(AddWire);

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
                    comment.Comment = $"The constant wire, value (base 10) 2^{i.ToString(CultureInfo.InvariantCulture)}";

                    var con = new ConstWireConstraint();
                    AddConstraint(con);

                    con.ConstVariableWires = new PinocchioVariableWires(powerOfTwoBaseVariable.RawVariable,
                        commonArg.PowerOfTwoWires[i]);

                    //todo: bug: the value of 2^254 is not correct. Figure out why.
                }
            }

            var outputVariableWiresDict = new Dictionary<RawVariable, (PinocchioTypeWires TypeWires, string VarName)>();

            void AddOutputVariableWires(PinocchioVariableWires variableWires, string varName)
            {
                if (!outputVariableWiresDict.ContainsKey(variableWires.RawVariable))
                {
                    outputVariableWiresDict.Add(variableWires.RawVariable, (TypeWires: new PinocchioTypeWires(variableWires), VarName: varName));
                }
                else
                {
                    Debug.Assert(varName == outputVariableWiresDict[variableWires.RawVariable].VarName);
                    Debug.Assert(variableWires.Wires.SequenceEqual(outputVariableWiresDict[variableWires.RawVariable].TypeWires.Wires));
                }
            }

            foreach (var node in this.VariableMap.TopologicalSort())
            {

                switch (node)
                {
                    case VariableNode variableNode:
                        // a variable node is either an input/nizk/constant node, which is NOT produced by constraints
                        // or a non-constant intermediate/output node, which has already been produced by constraints

                        PinocchioVariableWires outputVarWires = null;

                        if (!rawVarToWires.ContainsKey(variableNode.RawVariable))
                        {
                            // new wire(s)

                            Debug.Assert(
                                ((variableNode.NizkAttribute == NizkVariableType.Intermediate || variableNode.NizkAttribute == NizkVariableType.Output) && variableNode.RawVariable.Value.IsConstant) ||
                                variableNode.NizkAttribute == NizkVariableType.Input ||
                                variableNode.NizkAttribute == NizkVariableType.NizkInput);

                            if (variableNode.RawVariable.Value.IsConstant)
                            {
                                var output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                                AddPinocchioSubOutput(output);

                                {
                                    var comment = new CommentConstraint();
                                    AddConstraint(comment);
                                    comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wires are of a constant value (base 10) {variableNode.RawVariable.Type.GetVariableString(variableNode.RawVariable)}.";

                                    var con = new ConstWireConstraint();
                                    AddConstraint(con);
                                    con.ConstVariableWires = output.VariableWires;
                                }

                                if (variableNode.NizkAttribute == NizkVariableType.Output)
                                {
                                    outputVarWires = output.VariableWires;
                                }

                                // obsolete: mul by one is handled later
                                //// if the output variable is also constant, mul by one as new outputs
                                //if (variableNode.NizkAttribute == NizkVariableType.Output)
                                //{
                                //    List<PinocchioWire> outputWires = new List<PinocchioWire>();
                                //    foreach (var wire in output.VariableWires.Wires)
                                //    {
                                //        var newWire = new PinocchioWire();
                                //        AddWire(newWire);

                                //        var newCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                //        AddConstraint(newCon);

                                //        newCon.InWires.Add(wire);
                                //        newCon.InWires.Add(commonArg.OneWire);
                                //        newCon.OutWires.Add(newWire);

                                //        outputWires.Add(newWire);
                                //    }

                                //    outputVarWires = new PinocchioVariableWires(variableNode.RawVariable, outputWires);
                                //}

                            }
                            else
                            {
                                switch (variableNode.NizkAttribute)
                                {
                                    // policy: checkRange is applied for nizkinput, and not applied for others
                                    case NizkVariableType.NizkInput:
                                        {
                                            var comment = new CommentConstraint();
                                            AddConstraint(comment);

                                            var con = new UserPrivateInputConstraint();
                                            AddConstraint(con);

                                            var output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, true);
                                            AddPinocchioSubOutput(output);

                                            comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wires are nizk-input wires of variable \"{variableNode.VarName}\".";
                                            con.TypeWires = new PinocchioTypeWires(output.VariableWires);

                                            break;
                                        }
                                    case NizkVariableType.Input:
                                        {
                                            var output = variableNode.RawVariable.Type.VariableNodeToPinocchio(variableNode.RawVariable, commonArg, false);
                                            AddPinocchioSubOutput(output);

                                            var comment = new CommentConstraint();
                                            AddConstraint(comment);
                                            comment.Comment = $"The following {output.VariableWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wires are input wires of variable \"{variableNode.VarName}\".";

                                            var con = new UserInputConstraint();
                                            AddConstraint(con);
                                            con.TypeWires = new PinocchioTypeWires(output.VariableWires);
                                            break;
                                        }
                                    default:
                                        throw CommonException.AssertFailedException();
                                }
                            }

                        }
                        else
                        {
                            // old wire(s)

                            Debug.Assert(variableNode.NizkAttribute == NizkVariableType.Intermediate ||
                                         variableNode.NizkAttribute == NizkVariableType.Output);
                            Debug.Assert(!variableNode.RawVariable.Value.IsConstant);


                            if (variableNode.NizkAttribute == NizkVariableType.Output)
                            {
                                outputVarWires = rawVarToWires[variableNode.RawVariable];
                            }

                        }

                        if (outputVarWires != null)
                        {
                            AddOutputVariableWires(outputVarWires, variableNode.VarName);
                        }

                        break;
                    case OperationNode operationNode:
                        // get all in-variable
                        List<PinocchioVariableWires> inVars = new List<PinocchioVariableWires>();
                        foreach (var prevNode in operationNode.PrevNodes)
                        {
                            var varNode = (VariableNode)prevNode;
                            Debug.Assert(rawVarToWires.ContainsKey(varNode.RawVariable));

                            inVars.Add(rawVarToWires[varNode.RawVariable]);
                        }

                        // currently, assume at least one inVar
                        Debug.Assert(inVars.Count >= 1);

                        // for every outputVar, produce new PinocchioOutput according to the operation
                        foreach (var outNode in operationNode.NextNodes)
                        {
                            var outVarNode = (VariableNode)outNode;
                            PinocchioSubOutput output = inVars[0].RawVariable.Type.OperationNodeToPinocchio(
                                operationNode.ConnectionType,
                                inVars,
                                outVarNode.RawVariable,
                                commonArg
                            );

                            AddPinocchioSubOutput(output);
                        }
                        break;
                    default:
                        throw CommonException.AssertFailedException();
                }
            }

            // todo: optimize redundancy wire & constraints            
            // note: even if all the output of some constraints are not used, it is NOT considered useless
            // example here: input 1; split in 1 <1> out 32 <2,3,4,...,33>; this constraint ensure that "input 1" is smaller than 2^32, and must not be deleted while optimizing

            // declare the output wires at the end
            foreach (var (_, (typeWires, varName)) in outputVariableWiresDict)
            {
                var newTypeWires = new PinocchioTypeWires();
                newTypeWires.Type = typeWires.Type;

                // mul by 1
                foreach (var wire in typeWires.Wires)
                {
                    var newWire = new PinocchioWire();
                    AddWire(newWire);

                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                    AddConstraint(con);

                    con.InWires.Add(wire);
                    con.InWires.Add(commonArg.OneWire);
                    con.OutWires.Add(newWire);

                    newTypeWires.Wires.Add(newWire);
                    // ReSharper disable once UseIndexFromEndExpression
                    Debug.Assert(typeWires.Wires[newTypeWires.Wires.Count - 1] == wire);
                }

                {
                    var comment = new CommentConstraint();
                    AddConstraint(comment);
                    comment.Comment = $"The following {newTypeWires.Wires.Count.ToString(CultureInfo.InvariantCulture)} wires are output wires of variable \"{varName}\".";
                }

                {
                    var con = new OutputConstraint();
                    AddConstraint(con);
                    con.TypeWires = newTypeWires;
                }
            }

            // write these wire & constraints
            arithWriter.WriteLine("total " + (wireToID.Keys.Count.ToString(CultureInfo.InvariantCulture)));
            foreach (var constraint in conList)
            {
                switch (constraint)
                {
                    case BasicPinocchioConstraint basicPinocchioConstraint:
                        StringBuilder sb = new StringBuilder();
                        _ = basicPinocchioConstraint.Type switch
                        {
                            BasicPinocchioConstraintType.Add => sb.Append("add"),
                            BasicPinocchioConstraintType.Mul => sb.Append("mul"),
                            BasicPinocchioConstraintType.Or => sb.Append("or"),
                            BasicPinocchioConstraintType.Pack => sb.Append("pack"),
                            BasicPinocchioConstraintType.Split => sb.Append("split"),
                            BasicPinocchioConstraintType.Xor => sb.Append("xor"),
                            BasicPinocchioConstraintType.ZeroP => sb.Append("zerop"),
                            _ => throw CommonException.AssertFailedException(),
                        };

                        // zerop compatibility
                        if (basicPinocchioConstraint.Type == BasicPinocchioConstraintType.ZeroP &&
                            basicPinocchioConstraint.OutWires.Count == 1)
                        {
                            basicPinocchioConstraint.OutWires.Insert(0, commonArg.OneWire);
                        }

                        // zerop compatibility end


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

                        arithWriter.WriteLine(sb.ToString());

                        break;
                    case UserInputConstraint userInputConstraint:
                        Debug.Assert(userInputConstraint.TypeWires.Wires.Count != 0);
                        userInputConstraint.TypeWires.Wires.ForEach(wire => arithWriter.WriteLine("input " + wireToID[wire] + " #userinput"));
                        userInputConstraint.TypeWires.Wires.ForEach(wire => inHelperWriter.WriteLine(wireToID[wire] + " <to-be-filled-by-user, base 16>"));
                        break;
                    case UserPrivateInputConstraint userPrivateInputConstraint:
                        Debug.Assert(userPrivateInputConstraint.TypeWires.Wires.Count != 0);
                        userPrivateInputConstraint.TypeWires.Wires.ForEach(wire => arithWriter.WriteLine("nizkinput " + wireToID[wire] + " #userinput"));
                        userPrivateInputConstraint.TypeWires.Wires.ForEach(wire => inHelperWriter.WriteLine(wireToID[wire] + " <to-be-filled-by-user, base 16>"));
                        break;
                    case OutputConstraint outputConstraint:
                        Debug.Assert(outputConstraint.TypeWires.Wires.Count != 0);
                        outputConstraint.TypeWires.Wires.ForEach(wire => arithWriter.WriteLine("output " + wireToID[wire] + " #output"));
                        break;
                    case ConstWireConstraint constWireConstraint:
                        Debug.Assert(constWireConstraint.ConstVariableWires.Wires.Count != 0);
                        constWireConstraint.ConstVariableWires.Wires.ForEach(wire => arithWriter.WriteLine("input " + wireToID[wire] + " #const"));
                        if (constWireConstraint.ConstVariableWires.Wires.Count == 1)
                        {
                            constWireConstraint.ConstVariableWires.Wires.ForEach(wire => inHelperWriter.WriteLine(wireToID[wire] + " " + constWireConstraint.ConstVariableWires.RawVariable.Type.GetVariableInt(constWireConstraint.ConstVariableWires.RawVariable).ToString("X", CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            constWireConstraint.ConstVariableWires.Wires.ForEach(wire => inHelperWriter.WriteLine(wireToID[wire] + " <todo-multiple-wires-constant>"));
                        }

                        break;
                    case CommentConstraint commentConstraint:
                        arithWriter.WriteLine("#" + commentConstraint.Comment);
                        break;
                    case DivModConstraint divModConstraint:
                        //todo
                        throw new NotImplementedException();

                    default:
                        throw CommonException.AssertFailedException();
                }
            }

            // congrats!
        }




    }
}
