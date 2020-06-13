using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using code0k_cc.Config;
using code0k_cc.CustomException;
using code0k_cc.Pinocchio;
using code0k_cc.Pinocchio.Constraint;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.Type
{
    partial class NType
    {
        public static readonly NType Field = new NType("field")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = BigInteger.Zero,
                        }
                    };
                }
                else if (commonConstant == VariableCommonConstant.One)
                {
                    return new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = BigInteger.One,
                        }
                    };
                }
                else if (commonConstant == VariableCommonConstant.MinusOne)
                {
                    return new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = My.Config.ModulusPrimeField_Prime,
                        }
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.Field}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
            ParseFunc = (str) =>
            {
                if (BigInteger.TryParse(str, out BigInteger retV))
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = retV,
                        }
                    });
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.Field.TypeCodeName}\".");
                }
            },
            GetVariableStringFunc = variable => ( (NizkFieldValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            GetNewNizkVariableFunc = () => new Variable(new RawVariable()
            {
                Type = NType.Field,
                Value = new NizkFieldValue()
                {
                    IsConstant = false,
                    Value = BigInteger.Zero,
                }
            }),

            InternalConvertFunc = (variable, type) =>
            {
                var selfType = NType.Field;
                if (type == selfType)
                {
                    return variable;
                }

                var allowedType = new List<NType>() { NType.UInt32, NType.Bool, };
                if (!allowedType.Contains(type))
                {
                    throw new Exception($"Can't do internal convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }

                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }

                if (type == NType.UInt32)
                {
                    if (( (NizkFieldValue) variable.Value ).Value <= new BigInteger(System.UInt32.MaxValue))
                    {
                        return new Variable(new RawVariable()
                        {
                            Type = NType.UInt32,
                            Value = new NizkUInt32Value()
                            {
                                IsConstant = true,
                                Value = System.UInt32.Parse(( ( (NizkFieldValue) variable.Value ).Value ).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
                            }
                        });
                    }
                    else
                    {
                        throw new Exception($"Overflow detected while doing internal convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                    }

                }
                else if (type == NType.Bool)
                {
                    if (( (NizkFieldValue) variable.Value ).Value <= BigInteger.One)
                    {
                        return NType.Bool.GetCommonConstantValue(( (NizkFieldValue) variable.Value ).Value == BigInteger.One ? VariableCommonConstant.One : VariableCommonConstant.Zero);
                    }
                    else
                    {
                        throw new Exception($"Overflow detected while doing internal convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                    }
                }
                else
                {
                    throw CommonException.AssertFailedException();
                }
            },
            // Implicit Convert: not supported
            ExplicitConvertFunc = (variable, type) =>
            {
                var selfType = NType.Field;
                if (type == selfType)
                {
                    return variable;
                }

                var allowedType = new List<NType>() { NType.UInt32, NType.Bool, };
                if (!allowedType.Contains(type))
                {
                    throw new Exception($"Can't explicit convert \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }

                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }

                if (type == NType.UInt32)
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = System.UInt32.Parse(( ( (NizkFieldValue) variable.Value ).Value % ( new BigInteger(System.UInt32.MaxValue) + 1 ) ).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
                        }
                    });
                }
                else if (type == NType.Bool)
                {
                    return NType.Bool.GetCommonConstantValue(( (NizkUInt32Value) variable.Value ).Value % 2 == 0 ? VariableCommonConstant.One : VariableCommonConstant.Zero);
                }
                else
                {
                    throw CommonException.AssertFailedException();
                }
            },

            BinaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable, Variable>>()
            {
                {VariableOperationType.Binary_Addition, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Field);
                    var v1 = ((NizkFieldValue) newVar1.Value);
                    var v2 = ((NizkFieldValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkFieldValue() {
                                IsConstant = true,
                                Value = (v1.Value + v2.Value) % My.Config.ModulusPrimeField_Prime,
                            }
                            : new NizkFieldValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Subtract, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Field);
                    var v1 = ((NizkFieldValue) newVar1.Value);
                    var v2 = ((NizkFieldValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkFieldValue() {
                                IsConstant = true,
                                Value = (My.Config.ModulusPrimeField_Prime + v1.Value - v2.Value) % My.Config.ModulusPrimeField_Prime,
                            }
                            : new NizkFieldValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Multiplication, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Field);
                    var v1 = ((NizkFieldValue) newVar1.Value);
                    var v2 = ((NizkFieldValue) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkFieldValue() {
                                IsConstant = true,
                                Value = ( v1.Value * v2.Value) % My.Config.ModulusPrimeField_Prime,
                            }
                            : new NizkFieldValue() {
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
                            if (operationType == VariableOperationType.TypeCast_NoCheckRange)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(commonArg.OneWire);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.TypeCast_Trim)
                            {
                                if (outputVariable.Type == NType.Bool)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.ZeroP);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(inVars[0].Wires[0]);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }
                                else if (outputVariable.Type == NType.UInt32)
                                {
                                    var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                                    ret.Constraints.Add(splitCon);

                                    splitCon.InWires.Add(inVars[0].Wires[0]);
                                    //todo: [MaxPossibleValue] design "Variable.MaxPossibleValue" and minimize the number of bits here
                                    foreach (var _ in Enumerable.Range(0, My.Config.ModulusPrimeField_Prime_Bit))
                                    {
                                        var boolWire = new PinocchioWire();
                                        ret.AnonymousWires.Add(boolWire);

                                        // [boolWire01] 

                                        splitCon.OutWires.Add(boolWire);
                                    }

                                    var packCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Pack);
                                    ret.Constraints.Add(packCon);

                                    packCon.OutWires.Add(outputWire);
                                    for (int i = My.Config.ModulusPrimeField_Prime_Bit - 1; i <= My.Config.ModulusPrimeField_Prime_Bit - 32; --i)
                                    {
                                        var boolWire = new PinocchioWire();
                                        ret.AnonymousWires.Add(boolWire);

                                        // [boolWire01] 

                                        packCon.InWires.Add(boolWire);
                                    }

                                    return ret;
                                }
                                else
                                {
                                    break;
                                }
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
                        break;
                    case VariableOperationTypeType.Binary:
                        Debug.Assert(inVars.Count == 2);
                        Debug.Assert(inVars[0].Wires.Count == 1);
                        Debug.Assert(inVars[1].Wires.Count == 1);
                        if (operationType == VariableOperationType.Binary_Addition ||
                            operationType == VariableOperationType.Binary_Multiplication ||
                            operationType == VariableOperationType.Binary_Subtract)
                        {
                            PinocchioSubOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioSubOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }
                            if (operationType == VariableOperationType.Binary_Addition)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[1].Wires[0]);
                                con.OutWires.Add(outputWire);
                            }
                            else if (operationType == VariableOperationType.Binary_Multiplication)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[1].Wires[0]);
                                con.OutWires.Add(outputWire);
                            }
                            else if (operationType == VariableOperationType.Binary_Subtract)
                            {
                                // var3= var2 * (-1)
                                var con1 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con1);

                                con1.InWires.Add(inVars[1].Wires[0]);
                                con1.InWires.Add(commonArg.MinusOneWire);

                                var var3 = new PinocchioWire();
                                ret.AnonymousWires.Add(var3);

                                con1.OutWires.Add(var3);

                                // ret = var1 + var 3

                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(var3);
                                con.OutWires.Add(outputWire);
                            }
                            else
                            {
                                throw CommonException.AssertFailedException();
                            }

                            return ret;
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
