using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using code0k_cc.Pinocchio;
using code0k_cc.Runtime.ExeResult;

namespace code0k_cc.Runtime.Type
{
    partial class NType
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
            var rawVariable = this.GetCommonConstantValueFunc(commonConstant);
            Debug.Assert(rawVariable.Type == this);
            return new Variable(rawVariable);
        }

        private Func<VariableCommonConstant, RawVariable> GetCommonConstantValueFunc { get; set; }

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

        public string GetVariableString(RawVariable rawVariable)
        {
            Debug.Assert(rawVariable.Type == this);
            return this.GetVariableStringFunc(rawVariable);
        }

        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        private Func<RawVariable, string> GetVariableStringFunc;

        public BigInteger GetVariableInt(RawVariable rawVariable)
        {
            Debug.Assert(rawVariable.Type == this);
            return this.GetVariableIntFunc(rawVariable);
        }

        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        private Func<RawVariable, BigInteger> GetVariableIntFunc;

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
            Debug.Assert(retVariable.Type == targetType);
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

            if ((this.UnaryOperationFuncs?.ContainsKey(op)).GetValueOrDefault())
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
            if ((this.BinaryOperationFuncs?.ContainsKey(op)).GetValueOrDefault())
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

        #region Pinocchio Staffs

        public PinocchioSubOutput VariableNodeToPinocchio(RawVariable rawVariable, PinocchioCommonArg commonArg, bool checkRange)
        {
            Debug.Assert(rawVariable.Type == this);
            return this.VariableNodeToPinocchioFunc(rawVariable, commonArg, checkRange);
        }

        private Func<RawVariable, PinocchioCommonArg, bool, PinocchioSubOutput> VariableNodeToPinocchioFunc { get; set; }

        public PinocchioSubOutput OperationNodeToPinocchio(VariableOperationType operationType, List<PinocchioVariableWires> inVars, RawVariable outputRawVariable, PinocchioCommonArg commonArg)
        {
            // currently, assume there is at least one in-variable
            Debug.Assert(inVars[0].RawVariable.Type == this);
            return this.OperationNodeToPinocchioFunc(operationType, inVars, outputRawVariable, commonArg);
        }
        private Func<VariableOperationType, List<PinocchioVariableWires>, RawVariable, PinocchioCommonArg, PinocchioSubOutput> OperationNodeToPinocchioFunc { get; set; }

        #endregion

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

            this.GetVariableStringFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support String().");
            this.GetVariableIntFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Int().");
            this.ParseFunc = s => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Parse().");
            this.GetNewNizkVariableFunc = () => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
            this.VariableNodeToPinocchioFunc = (rawVariable, commonArg, checkRange) => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");
            this.OperationNodeToPinocchioFunc = (operationType, inVars, outputVariable, commonArg) => throw new Exception($"Type \"{this.TypeCodeName}\" is not nizk-compatible.");

            this.GetCommonConstantValueFunc = commonConstant => throw new Exception($"Type \"{ this.TypeCodeName}\" doesn't provide a constant for \"{commonConstant}\".");
            this.UnaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable>>();
            this.BinaryOperationFuncs = new Dictionary<VariableOperationType, Func<Variable, Variable, Variable>>();
        }


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
            yield return NType.Field;
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
            return !(op1 == op2);
        }

    }
}
