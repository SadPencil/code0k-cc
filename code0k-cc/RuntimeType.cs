using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class RuntimeType
    {
        public string Name;
        public Func<EnvironmentBlock, RuntimeValue, object, RuntimeValue> Execute;
        public Func<RuntimeValue, bool> GetBool;
        public Func<RuntimeValue, Int32> GetInt32;

        private RuntimeType() { }

        public static RuntimeType GetRuntimeType(string name)
        {
            foreach (var runtimeType in GetAll())
            {
                if (runtimeType.Name == name)
                {
                    return runtimeType;
                }
            }

            return null;
        }

        public static IEnumerable<RuntimeType> GetAll()
        {
            yield return Function;
            yield return String;
        }

        public static readonly RuntimeType String = new RuntimeType()
        {
            Name = "string",
            Execute = (block, value, arg) => value
        };

        public static readonly RuntimeType Void = new RuntimeType()
        {
            Name = "void",
            Execute = (block, value, arg) => value,
            GetBool = () => false
        };

        public static readonly RuntimeType Function = new RuntimeType()
        {
            Name = "function",
            Execute = (block, value, arg) =>
            {
                // prepare new environment
                var data = (TFunctionData)value.Data;
                EnvironmentBlock newBlock = new EnvironmentBlock()
                {
                    ParentBlock = block.LocateVariable(data.FunctionName),
                    ParseInstance = data.Instance,
                    ReturnBlock = block,
                };
                // load function arguments if available
                // todo check function arg types!
                if (arg != null)
                {
                    var args = (Dictionary<string, RuntimeValue>)arg;
                    //todo correct the name
                    foreach (var pair in args)
                    {
                        newBlock.Variables.Add(pair.Key, pair.Value);
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
