using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Operation;

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
            return this.NewValueFunc();
        }
        /// <summary>
        /// Method to get a new value. Throw exceptions.
        /// </summary>
        private Func<Variable> NewValueFunc;

        public Variable Parse(string str)
        {
            return this.ParseFunc(str);
        }
        /// <summary>
        /// Method to get a new value from a token string. Throw exceptions.
        /// </summary>
        private Func<string, Variable> ParseFunc;

        public string String(Variable var)
        {
            Debug.Assert(var.Type == this);
            return this.StringFunc(var);
        }
        /// <summary>
        /// Method to get a string from a value. Throw exceptions.
        /// </summary>
        private Func<Variable, string> StringFunc;

        /// <summary>
        /// Generics Type Lists. Can be null.
        /// </summary>
        public IReadOnlyList<NType> GenericsTypes { get; private set; }

        public Variable Assign(Variable var, NType type)
        {
            Debug.Assert(var.Type == this);
            return this.AssignFunc(var, type);
        }
        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        private Func<Variable, NType, Variable> AssignFunc;

        public Variable ImplicitConvert(Variable var, NType type)
        {
            Debug.Assert(var.Type == this);
            return this.ImplicitConvertFunc(var, type);
        }
        /// <summary>
        /// RightVal, LeftType, LeftVal
        /// </summary>
        private Func<Variable, NType, Variable> ImplicitConvertFunc;

        public Variable UnaryOperation(Variable var, UnaryOperation op)
        {
            Debug.Assert(var.Type == this);
            if (( this.UnaryOperations?.ContainsKey(op) ).GetValueOrDefault())
            {
                return this.UnaryOperations[op](var);
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }

        public IReadOnlyDictionary<UnaryOperation, Func<Variable, Variable>> UnaryOperations { get; private set; }
        public Variable BinaryOperation(Variable var, Variable another, BinaryOperation op)
        {
            Debug.Assert(var.Type == this);
            if (( this.BinaryOperations?.ContainsKey(op) ).GetValueOrDefault())
            {
                return this.BinaryOperations[op](var, another);
            }
            else
            {
                throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support \"{op.ToString()}\" operation.");
            }
        }
        public IReadOnlyDictionary<BinaryOperation, Func<Variable, Variable, Variable>> BinaryOperations { get; private set; }

        private NType(string TypeCodeName)
        {
            this.TypeCodeName = TypeCodeName;
            // default behaviors

            this.AssignFunc = this.ImplicitConvert;
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

            this.StringFunc = variable => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support String().");
            this.ParseFunc = s => throw new Exception($"Type \"{this.TypeCodeName}\" doesn't support Parse().");

        }

        public static readonly NType UInt32 = new NType("uint32")
        {
            NewValueFunc = () => new Variable() { Type = NType.UInt32, Value = System.UInt32.MinValue },
            ParseFunc = (str) =>
            {
                if (System.UInt32.TryParse(str, out uint retUint))
                {
                    return new Variable() { Type = UInt32, Value = System.UInt32.Parse(str) };
                }
                else
                {
                    throw new Exception($"Can't parse \"{str}\" as \"{NType.UInt32.TypeCodeName}\".");
                }
            },
            StringFunc = variable => ( (System.UInt32) variable.Value ).ToString(CultureInfo.InvariantCulture),
            GenericsTypes = null,
            UnaryOperations = new Dictionary<UnaryOperation, Func<Variable, Variable>>()
            {
                //todo write unary operation
            },
            BinaryOperations = new Dictionary<BinaryOperation, Func<Variable, Variable, Variable>>()
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
            //todo
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
            return this.TypeCodeName.GetHashCode();
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
