using System.Collections.Generic;
using code0k_cc.Lex;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExeArg;
using code0k_cc.Runtime.ExeResult;

namespace code0k_cc.Parse
{
    class ParseInstance
    {
        public ParseUnit ParseUnit = null;
        public Token Token = null;
        public IReadOnlyList<ParseInstance> Children;

        public ExeResult Execute(ExeArg arg)
        {
            arg.Instance = this;
            return this.ParseUnit.Execute(arg);
        }
    }
}
