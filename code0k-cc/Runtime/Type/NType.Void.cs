using System;

namespace code0k_cc.Runtime.Type
{
	partial class NType
    {
        public static readonly NType Void = new NType("void")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.Void,
                        Value = null
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.Void}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
        };

    }
}
