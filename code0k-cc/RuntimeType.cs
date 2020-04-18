using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace code0k_cc
{
    class RuntimeType
    {
        public string Name;
        public Func<EnvironmentBlock, RuntimeValue, RuntimeTypeExecuteArg, RuntimeValue> Execute;
        public Func<RuntimeValue, bool> GetBool;
        public Func<RuntimeValue, Int32> GetInt32;


        private RuntimeType() { }

        public static RuntimeType GetRuntimeType(string name)
        {
            return GetAll().FirstOrDefault(runtimeType => runtimeType.Name == name);
        }

        public static IEnumerable<RuntimeType> GetAll()
        {
            yield return Function;
            yield return StatementResult;

            yield return Void;
            yield return String;
        }

        public static readonly RuntimeType StatementResult = new RuntimeType()
        {
            Name = "StatementResult",
            //todo
        };

        public static readonly RuntimeType String = new RuntimeType()
        {
            Name = "String",
            Execute = (block, value, arg) => value
        };

        public static readonly RuntimeType Void = new RuntimeType()
        {
            Name = "Void",
            Execute = (block, value, arg) => value,
            GetBool = (value) => false
        };

        public static readonly RuntimeType Function = new RuntimeType()
        {
            Name = "Function",
            Execute = (block, value, arg) =>
            {
                // prepare new environment
                var data = (FunctionValueData) value.Data;
                EnvironmentBlock newBlock = new EnvironmentBlock()
                {
                    ParentBlock = block.LocateVariable(data.FunctionName),
                    ParseInstance = data.Instance,
                    ReturnBlock = block,
                };
                // load function arguments if available
                if (arg != null)
                {
                    var funcArg = (FunctionExecuteArg) arg;
                    if (data.ArgumentTypes.Count != funcArg.NameValues.Count)
                    {
                        throw new Exception($"Unexpected function arguments of function \"{data.FunctionName}\".");
                    }

                    foreach (var i in Enumerable.Range(0, ( (FunctionExecuteArg) arg ).NameValues.Count))
                    {
                        (string argName, RuntimeValue argValue) = ( (FunctionExecuteArg) arg ).NameValues[i];
                        if (data.ArgumentTypes[i] != argValue.Type)
                        {
                            throw new Exception($"Unexpected function argument \"{argName}\" of function \"{data.FunctionName}\".");
                        }
                        newBlock.Variables.Add(argName, argValue);

                    }
                }

                if (data.Instance == null)
                {
                    throw new Exception($"Unimplemented function \"{data.FunctionName}\"");
                }

                return data.Instance.Execute(newBlock, null);
            }
        };
    }
}
