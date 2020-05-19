using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace code0k_cc.Lex
{
    static class Lex
    {
        internal static IEnumerable<Token> Analyze(Stream stream)
        {
            return Analyze(stream, new UTF8Encoding(false));
        }
         
        internal static IEnumerable<Token> Analyze(Stream stream, Encoding encoding)
        {
            StreamReader reader = new StreamReader(stream, encoding);

            StringBuilder sb = new StringBuilder();
            LexState state = LexState.Empty;

            int row = 1;
            int column = 0;

            while (true)
            {
                LexChoice choice;

                Int32 nextCharInt = reader.Peek();
                char nextChar = Char.MinValue;
                LexCharType nextCharType = LexCharType.Unknown;

                Debug.Assert(( sb.Length == 0 ) == ( state == LexState.Empty ));

                if (nextCharInt == -1) // EOL
                {
                    if (state == LexState.Empty)
                    {
                        choice = LexChoice.Terminate;
                    }
                    else if (state == LexState.String || state == LexState.StringEscaping)
                    {
                        throw new Exception("Unexpected end of line. The string is not closed.");
                    }
                    else
                    {
                        choice = LexChoice.PeekReturn;
                    }
                }
                else
                {
                    nextChar = (char) nextCharInt;

                    ++column;
                    if (nextChar == '\n')
                    {
                        column = 0;
                        ++row;
                    }

                    nextCharType = GetCharType(nextChar);

                    switch (state) // a 5 by 6 table
                    {
                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.Unknown:
                        case LexState.Empty when nextCharType == LexCharType.Unknown:
                        case LexState.Punctuation when nextCharType == LexCharType.Unknown:
                            throw new Exception("Unrecognized character \"" + nextChar.ToString() + "\"");
                        case LexState.Empty when nextCharType == LexCharType.WhiteSpace:
                            choice = LexChoice.Drop;
                            break;


                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexState.Empty when nextCharType == LexCharType.QuotationMark:
                        case LexState.Empty when nextCharType == LexCharType.Backslash:
                        case LexState.Empty when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexState.Empty when nextCharType == LexCharType.OtherPunctuation:
                        case LexState.String when nextCharType == LexCharType.Unknown:
                        case LexState.String when nextCharType == LexCharType.WhiteSpace:
                        case LexState.String when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexState.String when nextCharType == LexCharType.OtherPunctuation:
                            choice = LexChoice.ReadAppend;
                            break;

                        case LexState.String when nextCharType == LexCharType.QuotationMark:
                            choice = LexChoice.ReadAppendReturn;
                            break;

                        case LexState.String when nextCharType == LexCharType.Backslash:
                            choice = LexChoice.ReadAppendStringEscapeIn;
                            break;

                        case LexState.StringEscaping when nextCharType == LexCharType.Unknown:
                        case LexState.StringEscaping when nextCharType == LexCharType.WhiteSpace:
                        case LexState.StringEscaping when nextCharType == LexCharType.Backslash:
                        case LexState.StringEscaping when nextCharType == LexCharType.QuotationMark:
                        case LexState.StringEscaping when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexState.StringEscaping when nextCharType == LexCharType.OtherPunctuation:
                            choice = LexChoice.ReadAppendStringEscapeOut;
                            break;

                        case LexState.Punctuation when nextCharType == LexCharType.WhiteSpace:
                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.WhiteSpace:
                            choice = LexChoice.DropReturn;
                            break;

                        case LexState.Punctuation when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.OtherPunctuation:
                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.Backslash:
                        case LexState.LetterOrDigitOrUnderscore when nextCharType == LexCharType.QuotationMark:
                            choice = LexChoice.PeekReturn;
                            break;

                        case LexState.Punctuation when nextCharType == LexCharType.Backslash:
                        case LexState.Punctuation when nextCharType == LexCharType.QuotationMark:
                        case LexState.Punctuation when nextCharType == LexCharType.OtherPunctuation:
                            // consider '&' and '&&'
                            if (GetTokenType(sb.ToString()) != null)
                            {
                                if (GetTokenType(sb.ToString() + nextChar) != null)
                                {
                                    choice = LexChoice.ReadAppend;
                                }
                                else
                                {
                                    choice = LexChoice.PeekReturn;
                                }
                            }
                            else
                            {
                                choice = LexChoice.ReadAppend;
                            }
                            break;
                        default:
                            throw CommonException.AssertFailedException();
                    }

                }

                switch (choice)
                {
                    case LexChoice.PeekReturn:
                        yield return GetToken(sb.ToString(), row, column);
                        _ = sb.Clear();
                        state = LexState.Empty;
                        break;
                    case LexChoice.DropReturn:
                        _ = reader.Read();
                        yield return GetToken(sb.ToString(), row, column);
                        _ = sb.Clear();
                        state = LexState.Empty;
                        break;
                    case LexChoice.Drop:
                        _ = reader.Read();
                        break;
                    case LexChoice.ReadAppendStringEscapeOut:
                    case LexChoice.ReadAppendReturn:
                    case LexChoice.ReadAppendStringEscapeIn:
                    case LexChoice.ReadAppend:
                        _ = reader.Read();
                        if (state == LexState.Empty)
                        {
                            switch (nextCharType)
                            {
                                case LexCharType.QuotationMark:
                                    state = LexState.String;
                                    break;
                                case LexCharType.LetterOrDigitOrUnderscore:
                                    state = LexState.LetterOrDigitOrUnderscore;
                                    break;
                                case LexCharType.Backslash:
                                case LexCharType.OtherPunctuation:
                                    state = LexState.Punctuation;
                                    break;
                                default:
                                    throw CommonException.AssertFailedException();
                            }
                        }
                        _ = sb.Append(nextChar);

                        if (choice == LexChoice.ReadAppendReturn)
                        {
                            yield return GetToken(sb.ToString(), row, column);
                            _ = sb.Clear();
                            state = LexState.Empty;
                        }
                        else if (choice == LexChoice.ReadAppendStringEscapeIn)
                        {
                            Debug.Assert(state == LexState.String);
                            state = LexState.StringEscaping;
                        }
                        else if (choice == LexChoice.ReadAppendStringEscapeOut)
                        {
                            Debug.Assert(state == LexState.StringEscaping);
                            state = LexState.String;
                        }

                        break;

                    case LexChoice.Terminate:
                        yield return GetEOL(row, column);
                        yield break;
                    default:
                        throw CommonException.AssertFailedException();
                }

            }

        }

        private static Token GetEOL(int row, int column)
        {
            return new Token() { Value = "", TokenType = TokenType.EOL, Row = row, Column = column };
        }

        private static TokenType GetTokenType(string word)
        {
            return TokenType.GetAll().FirstOrDefault(type => type.Match(word));
        }

        private static Token GetToken(string word, int row, int column)
        {
            TokenType tokenType = GetTokenType(word);
            return new Token() { Value = word, TokenType = tokenType, Row = row, Column = column };
        }

        private static LexCharType GetCharType(Char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                return LexCharType.WhiteSpace;
            }
            else if (Char.IsLetter(ch) || Char.IsDigit(ch) || ch == '_')
            {
                return LexCharType.LetterOrDigitOrUnderscore;
            }
            else if (ch == '\"')
            {
                return LexCharType.QuotationMark;
            }
            else if (ch == '\\')
            {
                return LexCharType.Backslash;
            }
            else if (( '!' <= ch && ch <= '/' || ':' <= ch && ch <= '@' || '[' <= ch && ch <= '`' || '{' <= ch && ch <= '~' ) && ch != '_' && ch != '\"')
            {
                return LexCharType.OtherPunctuation;
            }
            else
            {
                return LexCharType.Unknown;
            }
        }
    }
}
