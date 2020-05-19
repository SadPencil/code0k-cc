﻿using System;
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

        public Variable GetNewEmptyVariable()
        {
            var variable = this.GetNewEmptyVariableFunc();
            return variable;
        }

        /// <summary>
        /// Method to get a new value. Throw exceptions.
        /// </summary>
        private Func<Variable> GetNewEmptyVariableFunc;

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
            var newCon = new VariableConnection() { OperationType = VariableOperationType.TypeCast };
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
            var newCon = new VariableConnection() { OperationType = VariableOperationType.TypeCast };
            newCon.InVariables.Add(variable);
            newCon.OutVariables.Add(retVariable);

            retVariable.ParentConnections.Add(newCon);

            return retVariable;
        }

        /// <summary>
        /// RightVal, TargetType, TargetVal
        /// </summary>
        private Func<Variable, NType, Variable> ImplicitConvertFunc;

        /// <summary>
        /// RightVal, TargetType, TargetVal
        /// </summary>
        private Func<Variable, NType, Variable> ExplicitConvertFunc;

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

        public (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints) ToPinocchioWires(Variable variable, PinocchioCommonArg commonArg, bool checkRange)
        {
            Debug.Assert(variable.Type == this);
            return this.ToPinocchioWiresFunc(variable, commonArg, checkRange);
        }

        private Func<Variable, PinocchioCommonArg, bool, (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints)> ToPinocchioWiresFunc { get; set; }

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
                    throw new Exception($"Can't implicit convert \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
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
                    throw new Exception($"Can't explicit convert \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            };

            this.GetStringFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support String().");
            this.ParseFunc = s => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Parse().");
            this.GetNewNizkVariableFunc = () => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
            this.ToPinocchioWiresFunc = (variable, arg, checkRange) => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
        }

        public static readonly NType String = new NType("string")
        {
            GetNewEmptyVariableFunc = () => new Variable()
            {
                Type = NType.String,
                Value = new StringValue(),
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
                            throw new Exception("Assert failed!");
                    }
                }

                return new Variable()
                {
                    Type = NType.String,
                    Value = new StringValue() { Value = sb.ToString() },
                };
            },
        };

        public static readonly NType UInt32 = new NType("uint32")
        {
            GetNewEmptyVariableFunc = () => new Variable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = true,
                    Value = 0,

                }
            },
            ParseFunc = (str) =>
            {
                if (System.UInt32.TryParse(str, out System.UInt32 retV))
                {
                    return new Variable()
                    {
                        Type = NType.UInt32,
                        Value = new NizkUInt32Value()
                        {
                            IsConstant = true,
                            Value = retV,

                        }
                    };
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.UInt32.TypeCodeName}\".");
                }
            },
            GetStringFunc = variable => ( (NizkUInt32Value) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            GetNewNizkVariableFunc = () => new Variable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = false,
                    Value = 0,
                }
            },
            ExplicitConvertFunc = (variable, type) =>
            {
                if (NType.UInt32 == type)
                {
                    return variable;
                }
                else if (type == NType.Bool)
                {
                    if (variable.Value.IsConstant)
                    {
                        return new Variable()
                        {
                            Type = NType.Bool,
                            Value = new NizkBoolValue()
                            {
                                IsConstant = true,
                                Value = ( (NizkUInt32Value) variable.Value ).Value != 0,

                            }
                        };
                    }
                    else
                    {
                        return NType.Bool.GetNewNizkVariableFunc();
                    }

                }
                else
                {
                    throw new Exception($"Can't explicit convert \"{NType.UInt32.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            },
            UnaryOperationFuncs = new Dictionary<VariableOperationType , Func<Variable, Variable>>()
            {
                {VariableOperationType.Unary_Addition, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = + v1.Value ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Unary_Subtract, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = ((UInt32)0) - v1.Value ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Unary_BitwiseNot, (var1) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant  ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = ~ v1.Value ,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
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
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value+v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_Subtract, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value-v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_Multiplication, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value*v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_Division, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value/v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_Remainder, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value%v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_EqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value==v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_LessThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value<v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_LessEqualThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value<=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_GreaterThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value>v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_GreaterEqualThan, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value>=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_NotEqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value!=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,

                            }
                    };
                }},

                {VariableOperationType.Binary_BitwiseAnd, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value&v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_BitwiseOr, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value|v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_BitwiseXor, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.UInt32);
                    var v1 = ((NizkUInt32Value) newVar1.Value);
                    var v2 = ((NizkUInt32Value) newVar2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value^v2.Value,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                            }
                    };
                }},

                //todo implement Bitwise Shift
            },

            ToPinocchioWiresFunc = (variable, commonArg, checkRange) =>
            {
                var ret = (Wires: new List<PinocchioWire>(), Constraints: new List<PinocchioConstraint>());

                var uint32Value = variable.Value as NizkUInt32Value;
                if (uint32Value.IsConstant)
                {
                    ret.Wires.Add(new PinocchioWire(new BigInteger(uint32Value.Value)));
                }
                else
                {
                    var newWire = new PinocchioWire(null);
                    ret.Wires.Add(newWire);
                    if (checkRange)
                    {
                        var packCon = new PinocchioConstraint(PinocchioConstraintType.Pack);
                        ret.Constraints.Add(packCon);

                        packCon.InWires.Add(newWire);
                        foreach (var i in Enumerable.Range(0, 32))
                        {
                            var boolWire = new PinocchioWire(null);
                            ret.Wires.Add(boolWire);

                            var boolCon = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                            ret.Constraints.Add(boolCon);

                            boolCon.InWires.Add(boolWire);
                            boolCon.OutWires.Add(boolWire);

                            packCon.OutWires.Add(boolWire);
                        }
                    }
                }

                return ret;
            }
        };

        public static readonly NType Bool = new NType("bool")
        {
            GetNewEmptyVariableFunc = () => new Variable()
            {
                Type = NType.Bool,
                Value = new NizkBoolValue()
                {
                    IsConstant = true,
                    Value = false,
                }
            },
            ParseFunc = (str) =>
            {
                if (System.Boolean.TryParse(str, out System.Boolean retV))
                {
                    return new Variable()
                    {
                        Type = NType.Bool,
                        Value = new NizkBoolValue()
                        {
                            IsConstant = true,
                            Value = retV,
                        }
                    };
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.Bool.TypeCodeName}\".");
                }
            },
            GetStringFunc = variable => ( (NizkBoolValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            GetNewNizkVariableFunc = () => new Variable()
            {
                Type = NType.Bool,
                Value = new NizkBoolValue()
                {
                    IsConstant = false,
                    Value = false,
                }
            },
            ExplicitConvertFunc = (variable, type) =>
            {
                if (NType.Bool == type)
                {
                    return variable;
                }
                else if (type == NType.UInt32)
                {
                    if (variable.Value.IsConstant)
                    {
                        return new Variable()
                        {
                            Type = NType.UInt32,
                            Value = new NizkUInt32Value()
                            {
                                IsConstant = true,
                                Value = ( ( (NizkBoolValue) variable.Value ).Value ) ? (System.UInt32) 1 : (System.UInt32) 0,
                            }
                        };
                    }
                    else
                    {
                        return NType.UInt32.GetNewNizkVariableFunc();
                    }

                }
                else
                {
                    throw new Exception($"Can't explicit convert \"{NType.Bool.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            },
            UnaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable>>()
            {
                {VariableOperationType.Unary_BooleanNot, (var1) =>
                {
                    var v1 = ((NizkBoolValue) var1.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant  ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = ! v1.Value ,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
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
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value==v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_NotEqualTo, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value!=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_BooleanAnd, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value&&v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_BooleanOr, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value||v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

                {VariableOperationType.Binary_BooleanXor, (var1, var2) =>
                {
                    var newVar1 = var1;
                    var newVar2 = var2.Assign(NType.Bool);
                    var v1 = ((NizkBoolValue) newVar1.Value);
                    var v2 = ((NizkBoolValue) newVar2.Value);
                    return new Variable() {
                        Type = NType.Bool,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkBoolValue() {
                                IsConstant = true,
                                Value = v1.Value!=v2.Value,
                            }
                            : new NizkBoolValue() {
                                IsConstant = false,
                            }
                    };
                }},

            },

            ToPinocchioWiresFunc = (variable, commonArg, checkRange) =>
            {
                var ret = (Wires: new List<PinocchioWire>(), Constraints: new List<PinocchioConstraint>());

                var boolValue = variable.Value as NizkBoolValue;
                if (boolValue.IsConstant)
                {
                    ret.Wires.Add(new PinocchioWire(new BigInteger(boolValue.Value ? 1 : 0)));
                }
                else
                {
                    var newWire = new PinocchioWire(null);
                    ret.Wires.Add(newWire);

                    if (checkRange)
                    {
                        var boolCon = new PinocchioConstraint(PinocchioConstraintType.ZeroP);
                        ret.Constraints.Add(boolCon);

                        boolCon.InWires.Add(newWire);
                        boolCon.OutWires.Add(newWire);
                    }
                }

                return ret;
            }
        };

        public static readonly NType Void = new NType("void")
        {
            GetNewEmptyVariableFunc = () => new Variable() { Type = NType.Void, Value = null },
        };

        public static readonly NType Function = new NType("__Function")
        {
            GetNewEmptyVariableFunc = () => new Variable() { Type = NType.Function, Value = new FunctionDeclarationValue() },
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
