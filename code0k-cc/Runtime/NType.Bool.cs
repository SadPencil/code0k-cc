using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using code0k_cc.CustomException;
using code0k_cc.Pinocchio;
using code0k_cc.Pinocchio.Constraint;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime
{
    partial class NType
    {
        public static readonly NType Bool = new NType("bool")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.Bool,
                        Value = new NizkBoolValue()
                        {
                            IsConstant = true,
                            Value = false,
                        }
                    };
                }
                else if (commonConstant == VariableCommonConstant.One)
                {
                    return new RawVariable()
                    {
                        Type = NType.Bool,
                        Value = new NizkBoolValue()
                        {
                            IsConstant = true,
                            Value = true,
                        }
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.Bool}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
            ParseFunc = (str) =>
            {
                if (System.Boolean.TryParse(str, out System.Boolean retV))
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value = new NizkBoolValue()
                        {
                            IsConstant = true,
                            Value = retV,
                        }
                    });
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.Bool.TypeCodeName}\".");
                }
            },
            GetVariableStringFunc = variable => ( (NizkBoolValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            GetNewNizkVariableFunc = () => new Variable(new RawVariable()
            {
                Type = NType.Bool,
                Value = new NizkBoolValue()
                {
                    IsConstant = false,
                    Value = false,
                }
            }),
            InternalConvertFunc = (variable, type) => NType.Bool.ImplicitConvertFunc(variable, type),
            ImplicitConvertFunc = (variable, type) =>
            {
                var selfType = NType.Bool;
                if (type == selfType)
                {
                    return variable;
                }
                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }
                if (type == NType.Field)
                {
                    return NType.Field.GetCommonConstantValue(( (NizkBoolValue) variable.Value ).Value ? VariableCommonConstant.One : VariableCommonConstant.Zero);
                }
                else if (type == NType.UInt32)
                {
                    return NType.UInt32.GetCommonConstantValue(( (NizkBoolValue) variable.Value ).Value ? VariableCommonConstant.One : VariableCommonConstant.Zero);
                }
                else if (type == NType.Bool)
                {
                    throw CommonException.AssertFailedException();
                }
                else
                {
                    throw new Exception($"Can't convert \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            },
            ExplicitConvertFunc = (variable, type) => NType.Bool.ImplicitConvertFunc(variable, type),
            UnaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable>>()
            {
                {VariableOperationType.Unary_BooleanNot, (var1) =>
                {
                    var v1 = ((NizkBoolValue) var1.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant  ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = ! v1.Value ,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},
            },
            BinaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable, Variable>>()
            {
                {VariableOperationType.Binary_EqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value==v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_NotEqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value!=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_BooleanAnd, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value&&v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_BooleanOr, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value||v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_BooleanXor, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value!=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

            },

            VariableNodeToPinocchioFunc = (rawVariable, commonArg, checkRange) =>
            {
                var ret = new PinocchioSubOutput();

                var retWire = new PinocchioWire();
                ret.VariableWires = new PinocchioVariableWires(rawVariable, retWire);

                if (rawVariable.Value.IsConstant)
                {
                    //var con = new ConstWireConstraint();
                    //ret.Constraints.Add(con);
                    //con.ConstVariableWires = ret.VariableWires;
                }
                else
                {
                    if (checkRange)
                    {
                        // R * (R-1) = 0
                        var wire3 = new PinocchioWire();
                        ret.AnonymousWires.Add(wire3);

                        var addCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                        ret.Constraints.Add(addCon);

                        addCon.InWires.Add(retWire);
                        addCon.InWires.Add(commonArg.MinusOneWire);
                        addCon.OutWires.Add(wire3);

                        var mulCom = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                        ret.Constraints.Add(mulCom);

                        mulCom.InWires.Add(retWire);
                        mulCom.InWires.Add(wire3);
                        mulCom.OutWires.Add(commonArg.ZeroWire);
                    }
                }

                return ret;
            },


            OperationNodeToPinocchioFunc = (operationType, inVars, outputVariable, commonArg) =>
            {
                switch (operationType.Type)
                {
                    case VariableOperationTypeType.TypeCast:
                        Debug.Assert(inVars.Count == 1);
                        Debug.Assert(inVars[0].Wires.Count == 1);
                        if (operationType == VariableOperationType.TypeCast_NoCheckRange || operationType == VariableOperationType.TypeCast_Trim)
                        {
                            Debug.Assert(outputVariable.Type == NType.Field || outputVariable.Type == NType.UInt32 || outputVariable.Type == NType.Bool);

                            PinocchioSubOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioSubOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }
                            if (operationType == VariableOperationType.TypeCast_NoCheckRange ||
                            operationType == VariableOperationType.TypeCast_Trim && outputVariable.Type == NType.Field ||
                            operationType == VariableOperationType.TypeCast_Trim && outputVariable.Type == NType.UInt32)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(commonArg.OneWire);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else
                            {
                                throw CommonException.AssertFailedException();
                            }
                        }
                        else
                        {
                            break;
                        }
                    case VariableOperationTypeType.Unary:
                        Debug.Assert(inVars.Count == 1);
                        Debug.Assert(inVars[0].Wires.Count == 1);
                        if (operationType == VariableOperationType.Unary_BooleanNot)
                        {
                            Debug.Assert(outputVariable.Type == NType.Bool);

                            PinocchioSubOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioSubOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }

                            //operationType == VariableOperationType.Unary_BooleanNot
                            var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                            ret.Constraints.Add(con);

                            con.InWires.Add(inVars[0].Wires[0]);
                            con.InWires.Add(commonArg.OneWire);
                            con.OutWires.Add(outputWire);

                            return ret;
                        }
                        else
                        {
                            break;
                        }
                    case VariableOperationTypeType.Binary:
                        Debug.Assert(inVars.Count == 2);
                        Debug.Assert(inVars[0].Wires.Count == 1);
                        Debug.Assert(inVars[1].Wires.Count == 1);
                        if (operationType == VariableOperationType.Binary_EqualTo ||
                            operationType == VariableOperationType.Binary_NotEqualTo ||
                            operationType == VariableOperationType.Binary_BooleanAnd ||
                            operationType == VariableOperationType.Binary_BooleanOr ||
                            operationType == VariableOperationType.Binary_BooleanXor)
                        {
                            Debug.Assert(outputVariable.Type == NType.Bool);

                            PinocchioSubOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioSubOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }

                            if (operationType == VariableOperationType.Binary_BooleanAnd)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[0].Wires[1]);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Binary_BooleanOr)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Or);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[0].Wires[1]);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Binary_BooleanXor || operationType == VariableOperationType.Binary_NotEqualTo)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[0].Wires[1]);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Binary_EqualTo)
                            {
                                var wire1 = new PinocchioWire();
                                ret.AnonymousWires.Add(wire1);

                                var con1 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                ret.Constraints.Add(con1);

                                con1.InWires.Add(inVars[0].Wires[0]);
                                con1.InWires.Add(inVars[0].Wires[1]);
                                con1.OutWires.Add(wire1);

                                var con2 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                ret.Constraints.Add(con2);

                                con2.InWires.Add(wire1);
                                con2.InWires.Add(commonArg.OneWire);
                                con2.OutWires.Add(outputWire);

                                return ret;
                            }
                            else
                            {
                                throw CommonException.AssertFailedException();
                            }
                        }
                        else
                        {
                            break;
                        }
                    default:
                        throw CommonException.AssertFailedException();
                }
                throw new Exception($"Type \"{NType.Field}\" doesn't support \"{operationType.ToString()}\" operation.");

            },
        };

    }
}
