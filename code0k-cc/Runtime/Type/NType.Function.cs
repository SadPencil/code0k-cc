using System;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.Type
{
    partial class NType
    {
        public static readonly NType Function = new NType("__Function")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.Function,
                        Value = new FunctionDeclarationValue()
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.Function}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
        };
    }
}
