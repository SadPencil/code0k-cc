using System;
using System.Collections.Generic;
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
                var data = (FunctionValueData)value.Data;
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
                    //todo correct the name
                    foreach (var pair in ((FunctionExecuteArg)arg).NameValues)
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
