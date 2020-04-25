using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.Operation;
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

        public Variable NewValue()
        {
            var variable = this.NewValueFunc();
            return variable;
        }
        /// <summary>
        /// Method to get a new value. Throw exceptions.
        /// </summary>
        private Func<Variable> NewValueFunc;

        public Variable Parse(string str)
        {
            var variable = this.ParseFunc(str);
            return variable;
        }
        /// <summary>
        /// Method to get a new value from a token string. Throw exceptions.
        /// </summary>
        private Func<string, Variable> ParseFunc;

        public string String(Variable variable)
        {
            Debug.Assert(variable.Type == this);
            return this.StringFunc(variable);
        }
        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        private Func<Variable, string> StringFunc;

        /// <summary>
        /// Generics Type Lists. Can be null.
        /// </summary>
        public IReadOnlyList<NType> GenericsTypes { get; private set; }

        public Variable Assign(Variable variable, NType type)
        {
            return this.ImplicitConvert(variable, type);
        }

        ///// <summary>
        ///// RightVal, LeftType, LeftVal
        ///// </summary>
        //private Func<Variable, NType, Variable> AssignFunc;


        public Variable ExplicitConvert(Variable variable, NType type)
        {
            Debug.Assert(variable.Type == this);
            //todo detect whether two variable are same? or not?
            return this.ExplicitConvertFunc(variable, type);
        }

        public Variable ImplicitConvert(Variable variable, NType type)
        {
            Debug.Assert(variable.Type == this);
            //todo detect whether two variable are same? or not?
            return this.ImplicitConvertFunc(variable, type);
        }
        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        private Func<Variable, NType, Variable> ImplicitConvertFunc;
        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        private Func<Variable, NType, Variable> ExplicitConvertFunc;


        public Variable UnaryOperation(Variable variable, UnaryOperation op)
        {
            Debug.Assert(variable.Type == this);
            if (( this.UnaryOperationFuncs?.ContainsKey(op) ).GetValueOrDefault())
            {
                var retVariable = this.UnaryOperationFuncs[op](variable);

                // add connection
                var newCon = new VariableConnection() { Type = new VariableConnectionType() { UnaryOperation = op } };

                variable.Connections.Add(newCon);
                newCon.InVariables.Add(variable);

                newCon.OutVariables.Add(retVariable);

                // note: retVariable might be unused. The calculation of unused variables MUST be done, but the result will be cleared out later

                return retVariable;
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }

        private IReadOnlyDictionary<UnaryOperation, Func<Variable, Variable>> UnaryOperationFuncs { get; set; }
        public Variable BinaryOperation(Variable variable, Variable another, BinaryOperation op)
        {
            Debug.Assert(variable.Type == this);
            if (( this.BinaryOperationFuncs?.ContainsKey(op) ).GetValueOrDefault())
            {
                var retVariable = this.BinaryOperationFuncs[op](variable, another);
                // add connection
                var newCon = new VariableConnection() { Type = new VariableConnectionType() { BinaryOperation = op } };

                variable.Connections.Add(newCon);
                newCon.InVariables.Add(variable);

                another.Connections.Add(newCon);
                newCon.InVariables.Add(another);

                newCon.OutVariables.Add(retVariable);

                // note: retVariable might be unused. The calculation of unused variables MUST be done, but the result will be cleared out later

                return retVariable;
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }
        private IReadOnlyDictionary<BinaryOperation, Func<Variable, Variable, Variable>> BinaryOperationFuncs { get; set; }

        private NType(string TypeCodeName)
        {
            this.TypeCodeName = TypeCodeName;
            // default behaviors

            this.ImplicitConvertFunc = (variable, type) =>
            {
                Debug.Assert(variable.Type == this);
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
                Debug.Assert(variable.Type == this);
                if (variable.Type == type)
                {
                    return variable;
                }
                else
                {
                    throw new Exception($"Can't explicit convert \"{this.TypeCodeName }\" to \"{type.TypeCodeName}\".");
                }
            };

            this.StringFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support String().");
            this.ParseFunc = s => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Parse().");

        }

        public static readonly NType UInt32 = new NType("uint32")
        {
            NewValueFunc = () => new Variable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = true,
                    Value = 0,
                    VariableType = NizkVariableType.Intermediate
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
                            VariableType = NizkVariableType.Intermediate
                        }
                    };
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.UInt32.TypeCodeName}\".");
                }
            },
            StringFunc = variable => ( (NizkUInt32Value) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            UnaryOperationFuncs = new Dictionary<UnaryOperation, Func<Variable, Variable>>()
            {
                //todo write unary operation
            },
            BinaryOperationFuncs = new Dictionary<BinaryOperation, Func<Variable, Variable, Variable>>()
            {
                {Operation.BinaryOperation.Addition, (var1, var2) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    var v2 = ((NizkUInt32Value) var2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                            IsConstant = true,
                            Value = v1.Value+v2.Value,
                            VariableType = NizkVariableType.Intermediate,
                        }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                                VariableType = NizkVariableType.Intermediate,
                            }
                    };
                }},

                {Operation.BinaryOperation.Subtract, (var1, var2) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    var v2 = ((NizkUInt32Value) var2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value-v2.Value,
                                VariableType = NizkVariableType.Intermediate,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                                VariableType = NizkVariableType.Intermediate,
                            }
                    };
                }},

                {Operation.BinaryOperation.Multiplication, (var1, var2) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    var v2 = ((NizkUInt32Value) var2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value*v2.Value,
                                VariableType = NizkVariableType.Intermediate,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                                VariableType = NizkVariableType.Intermediate,
                            }
                    };
                }},

                {Operation.BinaryOperation.Division, (var1, var2) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    var v2 = ((NizkUInt32Value) var2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value/v2.Value,
                                VariableType = NizkVariableType.Intermediate,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                                VariableType = NizkVariableType.Intermediate,
                            }
                    };
                }},

                {Operation.BinaryOperation.Remainder, (var1, var2) =>
                {
                    var v1 = ((NizkUInt32Value) var1.Value);
                    var v2 = ((NizkUInt32Value) var2.Value);
                    return new Variable() {
                        Type = NType.UInt32,
                        Value =(v1.IsConstant && v2.IsConstant ) ?
                            new NizkUInt32Value() {
                                IsConstant = true,
                                Value = v1.Value%v2.Value,
                                VariableType = NizkVariableType.Intermediate,
                            }
                            : new NizkUInt32Value() {
                                IsConstant = false,
                                VariableType = NizkVariableType.Intermediate,
                            }
                    };
                }},


                //todo write binary operation
            },


        };

        public static readonly NType Bool = new NType("bool")
        {
            NewValueFunc = () => new Variable()
            {
                Type = NType.Bool,
                Value = new NizkBoolValue()
                {
                    IsConstant = true,
                    Value = false,
                    VariableType = NizkVariableType.Intermediate
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
                            VariableType = NizkVariableType.Intermediate
                        }
                    };
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.Bool.TypeCodeName}\".");
                }
            },
            StringFunc = variable => ( (NizkBoolValue) variable.Value ).Value.ToString(CultureInfo.InvariantCulture),
            UnaryOperationFuncs = new Dictionary<UnaryOperation, Func<Variable, Variable>>()
            {
                //todo write unary operation
            },
            BinaryOperationFuncs = new Dictionary<BinaryOperation, Func<Variable, Variable, Variable>>()
            {
                //todo write binary operation
            },

        };

        public static readonly NType Void = new NType("void")
        {
            NewValueFunc = () => new Variable() { Type = NType.Void, Value = null },
        };

        public static readonly NType Function = new NType("__Function")
        {
            NewValueFunc = () => new Variable() { Type = NType.Function, Value = new FunctionDeclarationValue() },
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
            yield return NType.Function;

            yield break;
        }

        public static IEnumerable<NType> GetGenericsNTypes()
        {
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
