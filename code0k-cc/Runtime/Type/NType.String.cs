using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using code0k_cc.CustomException;
using code0k_cc.Lex;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime
{
   partial class NType
    {
        public static readonly NType String = new NType("string")
        {
            GetCommonConstantValueFunc = commonConstant =>
            {
                if (commonConstant == VariableCommonConstant.Zero)
                {
                    return new RawVariable()
                    {
                        Type = NType.String,
                        Value = new StringValue(),
                    };
                }
                else
                {
                    throw new Exception($"Type \"{ NType.String}\" doesn't provide a constant for \"{commonConstant}\".");
                }
            },
            GetVariableStringFunc = variable => ( (StringValue) ( variable.Value ) ).Value,
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
    }
}
