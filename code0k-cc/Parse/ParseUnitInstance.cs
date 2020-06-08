using System.Collections.Generic;
using code0k_cc.Lex;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExeArg;
using code0k_cc.Runtime.ExeResult;

namespace code0k_cc.Parse
{
    class ParseUnitInstance
    {
        public ParseUnit ParseUnit = null;
        public Token Token = null;
        public IReadOnlyList<ParseUnitInstance> Children;

        public ExeResult Execute(ExeArg arg)
        {
            return this.ParseUnit.Execute(this, arg);
        }
    }
}
