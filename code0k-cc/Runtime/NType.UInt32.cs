using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using code0k_cc.CustomException;
using code0k_cc.Pinocchio;
using code0k_cc.Pinocchio.Constraint;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime
{
    partial class NType
    {
        public static readonly NType UInt32 = new NType("uint32")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = 0,
                        }
                    };
                }
                else if (commonConstant == VariableCommonConstant.One)
                {
                    return new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = 1,
                        }
                    };
                }
                else if (commonConstant == VariableCommonConstant.MinusOne)
                {
                    return new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = System.UInt32.MaxValue,
                        }
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.UInt32}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
            ParseFunc = (str) =>
            {
                if (System.UInt32.TryParse(str, out System.UInt32 retV))
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = retV,
                        }
                    });
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.UInt32.TypeCodeName}\".");
                }
            },
            GetVariableStringFunc = variable => ( (NizkUInt32Value) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            GetNewNizkVariableFunc = () => new Variable(new RawVariable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = false,
                    Value = 0,
                }
            }),
            InternalConvertFunc = (variable, type) =>
            {
                var selfType = NType.UInt32;
                if (type == selfType)
                {
                    return variable;
                }

                var allowedType = new List<NType>() { NType.Field, NType.Bool, };
                if (!allowedType.Contains(type))
                {
                    throw new Exception($"Can't do internal convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }

                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }

                if (type == NType.Field)
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = new BigInteger(( (NizkUInt32Value) variable.Value ).Value),
                        }
                    });
                }
                else if (type == NType.Bool)
                {
                    if (( (NizkUInt32Value) variable.Value ).Value <= 1)
                    {
                        return NType.Bool.GetCommonConstantValue(( (NizkUInt32Value) variable.Value ).Value == 1 ? VariableCommonConstant.One : VariableCommonConstant.Zero);
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
            ImplicitConvertFunc = (variable, type) =>
            {
                var selfType = NType.UInt32;
                if (type == selfType)
                {
                    return variable;
                }

                var allowedType = new List<NType>() { NType.Field };
                if (!allowedType.Contains(type))
                {
                    throw new Exception($"Can't do implicit convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }

                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }

                if (type == NType.Field)
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = new BigInteger(( (NizkUInt32Value) variable.Value ).Value),
                        }
                    });
                }
                else
                {
                    throw CommonException.AssertFailedException();
                }

            },
            ExplicitConvertFunc = (variable, type) =>
            {
                var selfType = NType.UInt32;
                if (type == selfType)
                {
                    return variable;
                }

                var allowedType = new List<NType>() { NType.Field, NType.Bool, };
                if (!allowedType.Contains(type))
                {
                    throw new Exception($"Can't do explicit convert from \"{selfType.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }

                if (!variable.Value.IsConstant)
                {
                    return type.GetNewNizkVariable();
                }

                if (type == NType.Field)
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = new BigInteger(( (NizkUInt32Value) variable.Value ).Value),
                        }
                    });
                }
                else if (type == NType.Bool)
                {
                    return NType.Bool.GetCommonConstantValue(( (NizkUInt32Value) variable.Value ).Value == 0 ? VariableCommonConstant.Zero : VariableCommonConstant.One);
                }
                else
                {
                    throw CommonException.AssertFailedException();
                }
            },
            UnaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable>>()
            {
                {VariableOperationType.Unary_Addition, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Unary_Subtract, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = Convert.ToUInt32((((UInt64)System.UInt32.MaxValue+1) + 0 - v1.Value) %  ((UInt64)System.UInt32.MaxValue+1) )  ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Unary_BitwiseNot, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = ~ v1.Value ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

            },

            BinaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable, Variable>>()
            {
                {VariableOperationType.Binary_Addition, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value =Convert.ToUInt32( ( (UInt64)v1.Value +v2.Value)%  ((UInt64)System.UInt32.MaxValue+1)) ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Subtract, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = Convert.ToUInt32((((UInt64)System.UInt32.MaxValue+1) +  v1.Value - v2.Value) %  ((UInt64)System.UInt32.MaxValue+1) )  ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Multiplication, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value =Convert.ToUInt32( ( (UInt64)v1.Value * v2.Value)%  ((UInt64)System.UInt32.MaxValue+1)) ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Division, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value/v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_Remainder, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value%v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_EqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
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

                {VariableOperationType.Binary_LessThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value<v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_LessEqualThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value<=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_GreaterThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value>v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_GreaterEqualThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value>=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_NotEqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
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

                {VariableOperationType.Binary_BitwiseAnd, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value&v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_BitwiseOr, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value|v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                {VariableOperationType.Binary_BitwiseXor, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value^v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    });
                }},

                //todo implement Bitwise Shift
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
                        var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                        ret.Constraints.Add(splitCon);

                        splitCon.InWires.Add(retWire);
                        foreach (var i in Enumerable.Range(0, 32))
                        {
                            var boolWire = new PinocchioWire();
                            ret.AnonymousWires.Add(boolWire);

                            //make sure boolWire is 0 or 1 (i.e. checkRange)
                            // [boolWire01] are "boolWire is 0 or 1" to be ensured here, or at Pinocchio Interface / Jsnark Interface?
                            // according to the source code of JsnarkInterface, it is ensured at Jsnark Interface, so the following codes are commented.
                            //{
                            //    var boolCon = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                            //    ret.Constraints.Add(boolCon);

                            //    boolCon.InWires.Add(boolWire);
                            //    boolCon.OutWires.Add(boolWire);
                            //}

                            splitCon.OutWires.Add(boolWire);
                        }
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
                            operationType == VariableOperationType.TypeCast_Trim && outputVariable.Type == NType.Field)
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
                        if (operationType == VariableOperationType.Unary_Addition ||
                            operationType == VariableOperationType.Unary_Subtract ||
                            operationType == VariableOperationType.Unary_BitwiseNot)
                        {
                            Debug.Assert(outputVariable.Type == NType.UInt32);

                            PinocchioSubOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioSubOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }

                            if (operationType == VariableOperationType.Unary_Addition)
                            {
                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(commonArg.OneWire);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Unary_Subtract)
                            {
                                // todo [LazyTrim]
                                // currently, assume the variable has already been trimmed every time an operation is done
                                // optimization can be done by implementing [MaxPossibleValue] 

                                var pow2_32 = commonArg.PowerOfTwoWires[32];

                                var negWire = new PinocchioWire();
                                ret.AnonymousWires.Add(negWire);

                                var con1 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                ret.Constraints.Add(con1);

                                con1.InWires.Add(inVars[0].Wires[0]);
                                con1.InWires.Add(commonArg.MinusOneWire);
                                con1.OutWires.Add(negWire);

                                var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                ret.Constraints.Add(con);

                                con.InWires.Add(pow2_32);
                                con.InWires.Add(negWire);
                                con.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Unary_BitwiseNot)
                            {
                                var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                                ret.Constraints.Add(splitCon);

                                splitCon.InWires.Add(inVars[0].Wires[0]);

                                var bitWires = new List<PinocchioWire>();
                                // todo [LazyTrim]
                                // currently, assume the variable has already been trimmed every time an operation is done
                                // optimization can be done by implementing [MaxPossibleValue]  
                                foreach (var i in Enumerable.Range(0, 32))
                                {
                                    var boolWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(boolWire);

                                    bitWires.Add(boolWire);

                                    // [boolWire01] 

                                    splitCon.OutWires.Add(boolWire);
                                }

                                var packCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Pack);
                                ret.Constraints.Add(packCon);

                                for (int i = 0; i < 32; i++)
                                {
                                    // [boolWire01] 

                                    var xorCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                    ret.Constraints.Add(xorCon);

                                    xorCon.InWires.Add(bitWires[i]);
                                    xorCon.InWires.Add(commonArg.OneWire);

                                    var newBitWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(newBitWire);

                                    xorCon.OutWires.Add(newBitWire);

                                    packCon.InWires.Add(newBitWire);
                                }

                                packCon.OutWires.Add(outputWire);

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
                    case VariableOperationTypeType.Binary:
                        Debug.Assert(inVars.Count == 2);
                        Debug.Assert(inVars[0].Wires.Count == 1);
                        Debug.Assert(inVars[1].Wires.Count == 1);
                        if (operationType == VariableOperationType.Binary_Addition ||
                            operationType == VariableOperationType.Binary_Subtract ||
                            operationType == VariableOperationType.Binary_Multiplication ||
                            operationType == VariableOperationType.Binary_Division ||
                            operationType == VariableOperationType.Binary_Remainder ||
                            operationType == VariableOperationType.Binary_EqualTo ||
                            operationType == VariableOperationType.Binary_LessThan ||
                            operationType == VariableOperationType.Binary_LessEqualThan ||
                            operationType == VariableOperationType.Binary_GreaterThan ||
                            operationType == VariableOperationType.Binary_GreaterEqualThan ||
                            operationType == VariableOperationType.Binary_NotEqualTo ||
                            operationType == VariableOperationType.Binary_BitwiseAnd ||
                            operationType == VariableOperationType.Binary_BitwiseOr ||
                            operationType == VariableOperationType.Binary_BitwiseXor)
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

                            if (operationType == VariableOperationType.Binary_BitwiseAnd ||
                                operationType == VariableOperationType.Binary_BitwiseOr ||
                                operationType == VariableOperationType.Binary_BitwiseXor)
                            {

                                var bitWires = new List<PinocchioWire>[2];
                                {
                                    for (int i = 0; i <= 1; ++i)
                                    {
                                        bitWires[i] = new List<PinocchioWire>();

                                        var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                                        ret.Constraints.Add(splitCon);

                                        splitCon.InWires.Add(inVars[i].Wires[0]);

                                        // todo [LazyTrim]
                                        // currently, assume the variable has already been trimmed every time an operation is done
                                        // optimization can be done by implementing [MaxPossibleValue]  
                                        foreach (var j in Enumerable.Range(0, 32))
                                        {
                                            var boolWire = new PinocchioWire();
                                            ret.AnonymousWires.Add(boolWire);

                                            bitWires[i].Add(boolWire);

                                            // [boolWire01] 

                                            splitCon.OutWires.Add(boolWire);
                                        }
                                    }
                                }

                                var packCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Pack);
                                ret.Constraints.Add(packCon);

                                for (int i = 0; i < 32; i++)
                                {
                                    // [boolWire01] 
                                    BasicPinocchioConstraint con;
                                    if (operationType == VariableOperationType.Binary_BitwiseAnd)
                                    {
                                        con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                        ret.Constraints.Add(con);
                                    }
                                    else if (operationType == VariableOperationType.Binary_BitwiseOr)
                                    {
                                        con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Or);
                                        ret.Constraints.Add(con);
                                    }
                                    else if (operationType == VariableOperationType.Binary_BitwiseXor)
                                    {
                                        con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                        ret.Constraints.Add(con);
                                    }
                                    else
                                    {
                                        throw CommonException.AssertFailedException();
                                    }

                                    con.InWires.Add(bitWires[0][i]);
                                    con.InWires.Add(bitWires[1][i]);

                                    var newBitWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(newBitWire);

                                    con.OutWires.Add(newBitWire);

                                    packCon.InWires.Add(newBitWire);
                                }

                                packCon.OutWires.Add(outputWire);

                                return ret;
                            }
                            else if (operationType == VariableOperationType.Binary_Addition ||
                                     operationType == VariableOperationType.Binary_Subtract ||
                                     operationType == VariableOperationType.Binary_Multiplication)
                            {
                                // todo [LazyTrim]
                                // currently, assume the variable has already been trimmed every time an operation is done
                                // optimization can be done by implementing [MaxPossibleValue] 

                                var wireToBeTrimmed = new PinocchioWire();
                                ret.AnonymousWires.Add(wireToBeTrimmed);

                                int wireToBeTrimmedMaxBit;

                                if (operationType == VariableOperationType.Binary_Addition)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(inVars[0].Wires[0]);
                                    con.InWires.Add(inVars[1].Wires[0]);
                                    con.OutWires.Add(wireToBeTrimmed);

                                    wireToBeTrimmedMaxBit = 32 + 1;
                                }
                                else if (operationType == VariableOperationType.Binary_Subtract)
                                {
                                    var pow2_32 = commonArg.PowerOfTwoWires[32];
                                    ret.AnonymousWires.Add(pow2_32);

                                    var negWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(negWire);

                                    {
                                        var con1 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                        ret.Constraints.Add(con1);

                                        con1.InWires.Add(inVars[1].Wires[0]);
                                        con1.InWires.Add(commonArg.MinusOneWire);
                                        con1.OutWires.Add(negWire);
                                    }

                                    var add1Con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                    ret.Constraints.Add(add1Con);

                                    add1Con.InWires.Add(pow2_32);
                                    add1Con.InWires.Add(inVars[0].Wires[0]);

                                    var addTempWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(addTempWire);

                                    add1Con.OutWires.Add(addTempWire);

                                    {
                                        var add2Con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                        ret.Constraints.Add(add2Con);

                                        add2Con.InWires.Add(addTempWire);
                                        add2Con.InWires.Add(negWire);
                                        add2Con.OutWires.Add(wireToBeTrimmed);
                                    }
                                    wireToBeTrimmedMaxBit = 32 + 2;
                                }
                                else if (operationType == VariableOperationType.Binary_Multiplication)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(inVars[0].Wires[0]);
                                    con.InWires.Add(inVars[1].Wires[0]);
                                    con.OutWires.Add(wireToBeTrimmed);

                                    wireToBeTrimmedMaxBit = 32 + 32;
                                }
                                else
                                {
                                    throw CommonException.AssertFailedException();
                                }

                                // now trim the wire

                                var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                                ret.Constraints.Add(splitCon);

                                splitCon.InWires.Add(wireToBeTrimmed);

                                var bitWires = new List<PinocchioWire>();

                                foreach (var i in Enumerable.Range(0, wireToBeTrimmedMaxBit))
                                {
                                    var boolWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(boolWire);

                                    bitWires.Add(boolWire);

                                    // [boolWire01] 

                                    splitCon.OutWires.Add(boolWire);
                                }

                                var packCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Pack);
                                ret.Constraints.Add(packCon);

                                for (int i = 0; i < 32; i++)
                                {
                                    // [boolWire01]  
                                    packCon.InWires.Add(bitWires[i]);
                                }

                                packCon.OutWires.Add(outputWire);

                                return ret;
                            }

                            else if (operationType == VariableOperationType.Binary_EqualTo ||
                                     operationType == VariableOperationType.Binary_LessThan ||
                                     operationType == VariableOperationType.Binary_LessEqualThan ||
                                     operationType == VariableOperationType.Binary_GreaterThan ||
                                     operationType == VariableOperationType.Binary_GreaterEqualThan ||
                                     operationType == VariableOperationType.Binary_NotEqualTo)
                            {
                                // todo [LazyTrim]
                                // currently, assume the variable has already been trimmed every time an operation is done
                                // optimization can be done by implementing [MaxPossibleValue] 

                                var subtractResultWire = new PinocchioWire();
                                ret.AnonymousWires.Add(subtractResultWire);
                                int subtractResultWireMaxBit;
                                {
                                    var pow2_32 = commonArg.PowerOfTwoWires[32];
                                    ret.AnonymousWires.Add(pow2_32);

                                    var negWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(negWire);

                                    var con1 = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con1);

                                    con1.InWires.Add(inVars[1].Wires[0]);
                                    con1.InWires.Add(commonArg.MinusOneWire);
                                    con1.OutWires.Add(negWire);

                                    var add1Con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                    ret.Constraints.Add(add1Con);

                                    add1Con.InWires.Add(pow2_32);
                                    add1Con.InWires.Add(inVars[0].Wires[0]);

                                    var addTempWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(addTempWire);

                                    add1Con.OutWires.Add(addTempWire);

                                    var add2Con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Add);
                                    ret.Constraints.Add(add2Con);

                                    add2Con.InWires.Add(addTempWire);
                                    add2Con.InWires.Add(negWire);
                                    add2Con.OutWires.Add(subtractResultWire);

                                    subtractResultWireMaxBit = 32 + 2;
                                }

                                PinocchioWire notEqualToResultBitWire;
                                {
                                    notEqualToResultBitWire = new PinocchioWire();
                                    ret.AnonymousWires.Add(notEqualToResultBitWire);

                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.ZeroP);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(subtractResultWire);
                                    con.OutWires.Add(notEqualToResultBitWire);
                                }

                                if (operationType == VariableOperationType.Binary_NotEqualTo)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(notEqualToResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }


                                PinocchioWire equalToResultBitWire = new PinocchioWire();
                                ret.AnonymousWires.Add(equalToResultBitWire);

                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(notEqualToResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(equalToResultBitWire);
                                }

                                if (operationType == VariableOperationType.Binary_EqualTo)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(equalToResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }

                                PinocchioWire greaterEqualThanResultBitWire;
                                {
                                    var splitCon = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Split);
                                    ret.Constraints.Add(splitCon);

                                    splitCon.InWires.Add(subtractResultWire);

                                    var bitWires = new List<PinocchioWire>();

                                    foreach (var i in Enumerable.Range(0, subtractResultWireMaxBit))
                                    {
                                        var boolWire = new PinocchioWire();
                                        ret.AnonymousWires.Add(boolWire);

                                        bitWires.Add(boolWire);

                                        // [boolWire01] 

                                        splitCon.OutWires.Add(boolWire);
                                    }

                                    greaterEqualThanResultBitWire = bitWires[32];
                                }

                                if (operationType == VariableOperationType.Binary_GreaterEqualThan)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(greaterEqualThanResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }



                                PinocchioWire lessThanResultBitWire = new PinocchioWire();
                                ret.AnonymousWires.Add(lessThanResultBitWire);

                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(greaterEqualThanResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(lessThanResultBitWire);
                                }


                                if (operationType == VariableOperationType.Binary_LessThan)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(lessThanResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }


                                PinocchioWire lessEqualThanResultBitWire = new PinocchioWire();
                                ret.AnonymousWires.Add(lessEqualThanResultBitWire);

                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Or);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(lessThanResultBitWire);
                                    con.InWires.Add(equalToResultBitWire);
                                    con.OutWires.Add(lessEqualThanResultBitWire);
                                }

                                if (operationType == VariableOperationType.Binary_LessEqualThan)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Mul);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(lessEqualThanResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }
                                else if (operationType == VariableOperationType.Binary_GreaterThan)
                                {
                                    var con = new BasicPinocchioConstraint(BasicPinocchioConstraintType.Xor);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(lessEqualThanResultBitWire);
                                    con.InWires.Add(commonArg.OneWire);
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }
                                else
                                {
                                    throw CommonException.AssertFailedException();
                                }
                            }
                            else if (operationType == VariableOperationType.Binary_Division ||
                                     operationType == VariableOperationType.Binary_Remainder)
                            {
                                //todo Implement Division and Remainder
                                throw new NotImplementedException();
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
                throw new Exception($"Type \"{NType.UInt32}\" doesn't support \"{operationType.ToString()}\" operation.");


            },


        };
    }
}
