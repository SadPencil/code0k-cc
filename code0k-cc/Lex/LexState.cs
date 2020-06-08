using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Lex
{
    enum LexState
    {
        Empty,
        Punctuation,
        LetterOrDigitOrUnderscore,
        String,
        StringEscaping,
    }
}
