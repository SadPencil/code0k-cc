using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using code0k_cc.Config;
using code0k_cc.CustomException;
using code0k_cc.Lex;
using code0k_cc.Pinocchio;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.ValueOfType;
using code0k_cc.Runtime.VariableMap;

namespace code0k_cc.Runtime
{
    class NType
    {
        /// <summary>
        /// The one and only one field to determine whether two types are actually the same.
        /// </summary>
        public string TypeCodeName { get; }

        public Variable GetEmptyVariable()
        {
            var variable = this.GetCommonConstantValue(VariableCommonConstant.Zero);
            return variable;
        }

        public Variable GetCommonConstantValue(VariableCommonConstant commonConstant)
        {
            if (this.CommonConstantValueDictionary.ContainsKey(commonConstant))
            {
                return new Variable(this.CommonConstantValueDictionary[commonConstant]);
            }
            else
            {
                throw new Exception($"Type \"{ this.TypeCodeName}\" doesn't provide a constant for \"{commonConstant}\".");
            }
        }

        private IReadOnlyDictionary<VariableCommonConstant, RawVariable> CommonConstantValueDictionary { get; set; }

        public Variable GetNewNizkVariable()
        {
            var variable = this.GetNewNizkVariableFunc();
            return variable;
        }

        /// <summary>
        /// Method to get a new nizk value. Throw exceptions.
        /// </summary>
        private Func<Variable> GetNewNizkVariableFunc;

        public Variable Parse(string str)
        {
            var variable = this.ParseFunc(str);
            return variable;
        }

        /// <summary>
        /// Method to get a new value from a token string. Throw exceptions.
        /// </summary>
        private Func<string, Variable> ParseFunc;

        public string GetString(Variable variable)
        {
            Debug.Assert(variable.Type == this);
            return this.GetStringFunc(variable);
        }

        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        private Func<Variable, string> GetStringFunc;

        /// <summary>
        /// Generics Type Lists. Can be null.
        /// </summary>
        public IReadOnlyList<NType> GenericsTypes { get; private set; }



        public Variable Assign(Variable variable, NType type)
        {
            return this.ImplicitConvert(variable, type);
        }

        public Variable ExplicitConvert(Variable variable, NType targetType)
        {
            Debug.Assert(variable.Type == this);
            var retVariable = this.ExplicitConvertFunc(variable, targetType);
            if (Object.ReferenceEquals(retVariable, variable))
            {
                return retVariable;
            }

            // add connection
            var newCon = new VariableConnection() { OperationType = VariableOperationType.TypeCast_Trim };
            newCon.InVariables.Add(variable);
            newCon.OutVariables.Add(retVariable);

            retVariable.ParentConnections.Add(newCon);

            return retVariable;
        }

        public Variable ImplicitConvert(Variable variable, NType targetType)
        {
            Debug.Assert(variable.Type == this);
            var retVariable = this.ImplicitConvertFunc(variable, targetType);
            if (Object.ReferenceEquals(retVariable, variable))
            {
                return retVariable;
            }

            // add connection
            var newCon = new VariableConnection() { OperationType = VariableOperationType.TypeCast_NoCheckRange };
            newCon.InVariables.Add(variable);
            newCon.OutVariables.Add(retVariable);

            retVariable.ParentConnections.Add(newCon);

            return retVariable;
        }


        public Variable InternalConvert(Variable variable, NType targetType)
        {
            Debug.Assert(variable.Type == this);
            var retVariable = this.InternalConvertFunc(variable, targetType);
            if (Object.ReferenceEquals(retVariable, variable))
            {
                return retVariable;
            }

            // add connection
            var newCon = new VariableConnection() { OperationType = VariableOperationType.TypeCast_NoCheckRange };
            newCon.InVariables.Add(variable);
            newCon.OutVariables.Add(retVariable);

            retVariable.ParentConnections.Add(newCon);

            return retVariable;
        }

        /// <summary>
        /// SelfVal, TargetType, TargetVal<br/>
        /// Implicit convert happens when a smaller type is assign to a larger type.<br/>
        /// Never allow implicit converting a larger type to a smaller type! 
        /// </summary>
        private Func<Variable, NType, Variable> ImplicitConvertFunc;

        /// <summary>
        /// SelfVal, TargetType, TargetVal<br/>
        /// Explicit convert happens when a smaller type is assign to a larger type, or vice versa.<br/>
        /// When a larger type is assign to a smaller type, TypeCast_Trim happens. 
        /// </summary>
        private Func<Variable, NType, Variable> ExplicitConvertFunc;

        /// <summary>
        /// SelfVal, TargetType, TargetVal<br/>
        /// Internal convert happens when a smaller type is assign to a larger type, or vice versa.<br/>
        /// When a larger type is assign to a smaller type, trim does NOT happens.<br/>
        /// Only use it when you are sure that the larger type's value is INDEED smaller than the maximum value of the smaller type. 
        /// </summary>
        private Func<Variable, NType, Variable> InternalConvertFunc;

        public Variable UnaryOperation(Variable variable, VariableOperationType op)
        {
            Debug.Assert(variable.Type == this);

            if (( this.UnaryOperationFuncs?.ContainsKey(op) ).GetValueOrDefault())
            {
                var retVariable = this.UnaryOperationFuncs[op](variable);

                // add connection
                var newCon = new VariableConnection() { OperationType = op };
                newCon.InVariables.Add(variable);
                newCon.OutVariables.Add(retVariable);

                retVariable.ParentConnections.Add(newCon);

                // note: retVariable might be unused. The calculation of unused variables MUST be done, but the result will be cleared out later

                return retVariable;
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }
        /// <summary>
        /// The variable's type is granted to be same as `this`
        /// </summary>
        private IReadOnlyDictionary<VariableOperationType, Func<Variable, Variable>> UnaryOperationFuncs { get; set; }
        public Variable BinaryOperation(Variable variable, Variable another, VariableOperationType op)
        {
            Debug.Assert(variable.Type == this);
            if (( this.BinaryOperationFuncs?.ContainsKey(op) ).GetValueOrDefault())
            {
                var retVariable = this.BinaryOperationFuncs[op](variable, another);
                // add connection
                var newCon = new VariableConnection() { OperationType = op };
                newCon.InVariables.Add(variable);
                newCon.InVariables.Add(another);
                newCon.OutVariables.Add(retVariable);

                retVariable.ParentConnections.Add(newCon);

                // note: retVariable might be unused. The calculation of unused variables MUST be done, but the result will be cleared out later

                return retVariable;
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }
        /// <summary>
        /// The first variable's type is granted to be same as `this`
        /// While the second variable's type can be anything
        /// </summary>
        private IReadOnlyDictionary<VariableOperationType, Func<Variable, Variable, Variable>> BinaryOperationFuncs { get; set; }

        // BEGIN pinocchio staffs
        public PinocchioOutput VariableNodeToPinocchio(RawVariable rawVariable, PinocchioCommonArg commonArg, bool checkRange)
        {
            Debug.Assert(rawVariable.Type == this);
            return this.VariableNodeToPinocchioFunc(rawVariable, commonArg, checkRange);
        }

        private Func<RawVariable, PinocchioCommonArg, bool, PinocchioOutput> VariableNodeToPinocchioFunc { get; set; }

        public PinocchioOutput OperationNodeToPinocchio(VariableOperationType operationType, List<PinocchioVariableWires> inVars, RawVariable outputRawVariable, PinocchioCommonArg commonArg)
        {
            // currently, assume there is at least one in-variable
            Debug.Assert(inVars[0].RawVariable.Type == this);
            return this.OperationNodeToPinocchioFunc(operationType, inVars, outputRawVariable, commonArg);
        }
        private Func<VariableOperationType, List<PinocchioVariableWires>, RawVariable, PinocchioCommonArg, PinocchioOutput> OperationNodeToPinocchioFunc { get; set; }
        // END pinocchio staffs

        private NType(string TypeCodeName)
        {
            this.TypeCodeName = TypeCodeName;
            // default behaviors

            this.ImplicitConvertFunc = (variable, type) =>
            {
                if (variable.Type == type)
                {
                    return variable;
                }
                else
                {
                    throw new Exception($"Can't do implicit convert from \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            };

            this.ExplicitConvertFunc = (variable, type) =>
            {
                if (variable.Type == type)
                {
                    return variable;
                }
                else
                {
                    throw new Exception($"Can't do explicit convert from \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            };

            this.InternalConvertFunc = (variable, type) =>
            {
                if (variable.Type == type)
                {
                    return variable;
                }
                else
                {
                    throw new Exception($"Can't do internal convert from \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            };

            this.GetStringFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support String().");
            this.ParseFunc = s => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Parse().");
            this.GetNewNizkVariableFunc = () => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
            this.VariableNodeToPinocchioFunc = (rawVariable, commonArg, checkRange) => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
            this.OperationNodeToPinocchioFunc = (operationType, inVars, outputVariable, commonArg) => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");

            this.CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>();
            this.UnaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable>>();
            this.BinaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable, Variable>>();
        }

        public static readonly NType String = new NType("string")
        {
            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.Zero, new RawVariable()
                    {
                        Type = NType.String,
                        Value = new StringValue(),
                    }
                },
            },
            GetStringFunc = variable => ( (StringValue) ( variable.Value ) ).Value,
            ParseFunc = str =>
            {
                Debug.Assert(str.First() == '\"');
                Debug.Assert(str.Last() == '\"');
                Debug.Assert(str.Length >= 2);

                StringBuilder sb = new StringBuilder();
                LexState state = LexState.String;
                foreach (var i in Enumerable.Range(1, str.Length - 2))
                {
                    char ch = str[i];
                    switch (state)
                    {
                        case LexState.StringEscaping:
                            _ = sb.Append(ch switch
                            {
                                'n' => '\n',
                                'r' => '\r',
                                't' => '\t',
                                '\\' => '\\',
                                '\"' => '\"',
                                '\'' => '\'',
                                '?' => '?',
                                _ => throw new Exception($"Unrecognized character \"\\{ch}\"")
                            });
                            state = LexState.String;
                            continue;

                        case LexState.String when ch == '\\':
                            state = LexState.StringEscaping;
                            continue;
                        case LexState.String when ch != '\\':
                            _ = sb.Append(ch);
                            continue;
                        default:
                            throw CommonException.AssertFailedException();
                    }
                }

                return new Variable(new RawVariable()
                {
                    Type = NType.String,
                    Value = new StringValue() { Value = sb.ToString() },
                });
            },
        };

        public static readonly NType Field = new NType("field")
        {

            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.Zero, new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = BigInteger.Zero,
                        }
                    }
                },
                {VariableCommonConstant.One, new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = BigInteger.One,
                        }
                    }
                },
                {VariableCommonConstant.MinusOne, new RawVariable()
                    {
                        Type = NType.Field,
                        Value = new NizkFieldValue()
                        {
                            IsConstant = true,
                            Value = My.Config.ModulusPrimeField_Prime - 1,
                        }
                    }
                },
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
            GetStringFunc = variable => ( (NizkFieldValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
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
                    return selfType.GetNewNizkVariable();
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
                                Value = System.UInt32.Parse(( ( (NizkFieldValue) variable.Value ).Value ).ToString()),
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
                    return selfType.GetNewNizkVariable();
                }

                if (type == NType.UInt32)
                {
                    return new Variable(new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = System.UInt32.Parse(( ( (NizkFieldValue) variable.Value ).Value % ( new BigInteger(System.UInt32.MaxValue) + 1 ) ).ToString()),
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
                var ret = new PinocchioOutput() { VariableWires = new PinocchioVariableWires(rawVariable) };

                var fieldValue = (NizkFieldValue) rawVariable.Value;
                if (fieldValue.IsConstant)
                {
                    ret.VariableWires.Wires.Add(new PinocchioWire(fieldValue.Value));
                }
                else
                {
                    var newWire = new PinocchioWire(null);
                    ret.VariableWires.Wires.Add(newWire);
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
                            PinocchioOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }
                            if (operationType == VariableOperationType.TypeCast_NoCheckRange)
                            {
                                var con = new PinocchioConstraint(PinocchioConstraintType.Mul);
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
                                    var con = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                                    ret.Constraints.Add(con);

                                    con.InWires.Add(inVars[0].Wires[0]); 
                                    con.OutWires.Add(outputWire);

                                    return ret;
                                }
                                else if (outputVariable.Type == NType.UInt32)
                                {
                                    var splitCon = new PinocchioConstraint(PinocchioConstraintType.Split);
                                    ret.Constraints.Add(splitCon);
                                    
                                    splitCon.InWires.Add(inVars[0].Wires[0]);
                                    //todo: design "Variable.MaxPossibleValue" and minimize the number of bits here
                                    foreach (var i in Enumerable.Range(0, My.Config.ModulusPrimeField_Prime_Bit))
                                    {
                                        var boolWire = new PinocchioWire(null);
                                        ret.AnonymousWires.Add(boolWire);
                                         
                                        //todo: [boolWire01] 

                                        splitCon.OutWires.Add(boolWire);
                                    }

                                    var packCon = new PinocchioConstraint(PinocchioConstraintType.Pack);
                                    ret.Constraints.Add(packCon);

                                    packCon.OutWires.Add(outputWire);
                                    for (int i = My.Config.ModulusPrimeField_Prime_Bit - 1; i <= My.Config.ModulusPrimeField_Prime_Bit - 32; --i)
                                    {
                                        var boolWire = new PinocchioWire(null);
                                        ret.AnonymousWires.Add(boolWire);

                                        //todo: [boolWire01] 

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
                            Debug.Assert(inVars[0].Wires.Count == 1);
                            Debug.Assert(inVars[1].Wires.Count == 1);

                            PinocchioOutput ret;
                            PinocchioWire outputWire;
                            {
                                var outVarWires = outputVariable.Type.VariableNodeToPinocchio(outputVariable, commonArg, false);
                                Debug.Assert(outVarWires.VariableWires.Wires.Count == 1);
                                outputWire = outVarWires.VariableWires.Wires[0];

                                ret = new PinocchioOutput() { VariableWires = outVarWires.VariableWires };
                                outVarWires.Constraints.ForEach(ret.Constraints.Add);
                            }
                            if (operationType == VariableOperationType.Binary_Addition)
                            {
                                var con = new PinocchioConstraint(PinocchioConstraintType.Add);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[1].Wires[0]);
                                con.OutWires.Add(outputWire);
                            }
                            else if (operationType == VariableOperationType.Binary_Multiplication)
                            {
                                var con = new PinocchioConstraint(PinocchioConstraintType.Mul);
                                ret.Constraints.Add(con);

                                con.InWires.Add(inVars[0].Wires[0]);
                                con.InWires.Add(inVars[1].Wires[0]);
                                con.OutWires.Add(outputWire);
                            }
                            else if (operationType == VariableOperationType.Binary_Subtract)
                            {
                                // var3= var2 * (-1)
                                var con1 = new PinocchioConstraint(PinocchioConstraintType.Mul);
                                ret.Constraints.Add(con1);

                                con1.InWires.Add(inVars[1].Wires[0]);
                                con1.InWires.Add(commonArg.MinusOneWire);

                                var var3 = new PinocchioWire(null);
                                ret.AnonymousWires.Add(var3);

                                con1.OutWires.Add(var3);

                                // ret = var1 + var 3

                                var con = new PinocchioConstraint(PinocchioConstraintType.Add);
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

        public static readonly NType UInt32 = new NType("uint32")
        {
            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.Zero, new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = 0,
                        }
                    }
                },
                {VariableCommonConstant.One, new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = 1,
                        }
                    }
                },
                {VariableCommonConstant.MinusOne, new RawVariable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = System.UInt32.MaxValue,
                        }
                    }
                },
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
            GetStringFunc = variable => ( (NizkUInt32Value) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
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
                    return selfType.GetNewNizkVariable();
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
                    return selfType.GetNewNizkVariable();
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
                    return selfType.GetNewNizkVariable();
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
                var ret = new PinocchioOutput() { VariableWires = new PinocchioVariableWires(rawVariable) };

                var uint32Value = (NizkUInt32Value) rawVariable.Value;
                if (uint32Value.IsConstant)
                {
                    ret.VariableWires.Wires.Add(new PinocchioWire(new BigInteger(uint32Value.Value)));
                }
                else
                {
                    var newWire = new PinocchioWire(null);
                    ret.VariableWires.Wires.Add(newWire);

                    if (checkRange)
                    {
                        var packCon = new PinocchioConstraint(PinocchioConstraintType.Split);
                        ret.Constraints.Add(packCon);

                        packCon.InWires.Add(newWire);
                        foreach (var i in Enumerable.Range(0, 32))
                        {
                            var boolWire = new PinocchioWire(null);
                            ret.AnonymousWires.Add(boolWire);

                            //make sure boolWire is 0 or 1 (i.e. checkRange)
                            //todo: [boolWire01] are "boolWire is 0 or 1" to be ensured here, or at Pinocchio Interface / Jsnark Interface?
                            //currently, assume this is ensured at Pinocchio Interface, so the following codes are commented.
                            //{
                            //    var boolCon = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                            //    ret.Constraints.Add(boolCon);

                            //    boolCon.InWires.Add(boolWire);
                            //    boolCon.OutWires.Add(boolWire);
                            //}

                            packCon.OutWires.Add(boolWire);
                        }
                    }
                }

                return ret;
            },

            OperationNodeToPinocchioFunc = (operationType, inVars, outputVariable, commonArg) =>
            {
                //todo 

            },


        };

        public static readonly NType Bool = new NType("bool")
        {

            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.One, new RawVariable()
                {
                    Type = NType.Bool,
                    Value = new NizkBoolValue()
                    {
                        IsConstant = true,
                        Value = true,
                    }
                }},
                {VariableCommonConstant.Zero, new RawVariable()
                {
                    Type = NType.Bool,
                    Value = new NizkBoolValue()
                    {
                        IsConstant = true,
                        Value = false,
                    }
                }},
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
            GetStringFunc = variable => ( (NizkBoolValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
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
                    return selfType.GetNewNizkVariable();
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
                var ret = new PinocchioOutput() { VariableWires = new PinocchioVariableWires(rawVariable) };

                var boolValue = (NizkBoolValue) rawVariable.Value;
                if (boolValue.IsConstant)
                {
                    ret.VariableWires.Wires.Add(new PinocchioWire(new BigInteger(boolValue.Value ? 1 : 0)));
                }
                else
                {
                    var newWire = new PinocchioWire(null);
                    ret.VariableWires.Wires.Add(newWire);

                    if (checkRange)
                    {
                        var boolCon = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                        ret.Constraints.Add(boolCon);

                        boolCon.InWires.Add(newWire);
                        boolCon.OutWires.Add(newWire);
                    }
                }

                return ret;
            },


            OperationNodeToPinocchioFunc = (operationType, inVars, outputVariable, commonArg) =>
            {

            },
        };

        public static readonly NType Void = new NType("void")
        {
            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.Zero,  new RawVariable() { Type = NType.Void, Value = null }}
            },
        };

        public static readonly NType Function = new NType("__Function")
        {
            CommonConstantValueDictionary = new Dictionary<VariableCommonConstant, RawVariable>()
            {
                {VariableCommonConstant.Zero,  new RawVariable() { Type = NType.Function, Value = new FunctionDeclarationValue() }}
            },
        };

        public static NType GetNType(TypeResult r)
        {
            if (r.Generics?.Types?.Count > 0)
            {
                //todo make generics type
                throw new NotImplementedException();
            }
            else
            {
                foreach (var nt in GetNonGenericsNTypes())
                {
                    if (r.TypeName == nt.TypeCodeName)
                    {
                        return nt;
                    }
                }
                throw new Exception($"Unknown type \"{r.TypeName}\"");
            }
        }

        public static IEnumerable<NType> GetNonGenericsNTypes()
        {
            yield return NType.Void;
            yield return NType.UInt32;
            yield return NType.Bool;
            yield return NType.String;
            yield return NType.Function;

            yield break;
        }

        public static IEnumerable<NType> GetGenericsNTypes()
        {
            throw new NotImplementedException();
            yield break;
        }


        public override string ToString()
        {
            return this.TypeCodeName.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is NType ntype)
            {
                return this.TypeCodeName == ntype.TypeCodeName;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.TypeCodeName);
        }

        public static bool operator ==(NType op1, NType op2)
        {
            return op1?.TypeCodeName == op2?.TypeCodeName;
        }
        public static bool operator !=(NType op1, NType op2)
        {
            return !( op1 == op2 );
        }

    }
}
