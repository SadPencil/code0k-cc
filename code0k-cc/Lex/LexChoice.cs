namespace code0k_cc.Lex
{
    enum LexChoice
    {
        Drop,
        DropReturn,
        PeekReturn,
        ReadAppend,
        ReadAppendReturn,
        ReadAppendStringEscapeIn,
        ReadAppendStringEscapeOut,
        Terminate
    }
}
