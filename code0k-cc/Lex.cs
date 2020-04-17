using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;

namespace code0k_cc
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
            LexCharType state = LexCharType.WhileSpace;

            while (true)
            {
                LexChoice choice;

                Int32 nextCharInt = reader.Peek();
                char nextChar = Char.MinValue;
                LexCharType nextCharType = LexCharType.Unknown;

                Debug.Assert((sb.Length == 0) == (state == LexCharType.WhileSpace));

                if (nextCharInt == -1) // EOL
                {
                    if (state == LexCharType.WhileSpace)
                    {
                        choice = LexChoice.Terminate;
                    }
                    else
                    {
                        choice = LexChoice.PeekReturn;
                    }
                }
                else
                {
                    nextChar = (char)nextCharInt;
                    nextCharType = GetCharType(nextChar);
                    if (nextCharType == LexCharType.Unknown)
                    {
                        throw new Exception("Unrecognized character \"" + nextChar.ToString() + "\"");
                    }

                    switch (state)
                    {
                        case LexCharType.WhileSpace when nextCharType == LexCharType.WhileSpace:
                            choice = LexChoice.Drop;
                            break;

                        case LexCharType.LetterOrDigitOrUnderscore when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexCharType.WhileSpace when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexCharType.WhileSpace when nextCharType == LexCharType.Punctuation:
                            choice = LexChoice.ReadAppend;
                            break;

                        case LexCharType.Punctuation when nextCharType == LexCharType.WhileSpace:
                        case LexCharType.LetterOrDigitOrUnderscore when nextCharType == LexCharType.WhileSpace:
                            choice = LexChoice.DropReturn;
                            break;

                        case LexCharType.Punctuation when nextCharType == LexCharType.LetterOrDigitOrUnderscore:
                        case LexCharType.LetterOrDigitOrUnderscore when nextCharType == LexCharType.Punctuation:
                            choice = LexChoice.PeekReturn;
                            break;

                        case LexCharType.Punctuation when nextCharType == LexCharType.Punctuation:
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
                            throw new Exception("Assert failed!");
                    }

                }

                switch (choice)
                {
                    case LexChoice.PeekReturn:
                        yield return GetToken(sb.ToString());
                        sb.Clear();
                        state = LexCharType.WhileSpace;
                        break;
                    case LexChoice.DropReturn:
                        reader.Read();
                        yield return GetToken(sb.ToString());
                        sb.Clear();
                        state = LexCharType.WhileSpace;
                        break;
                    case LexChoice.Drop:
                        reader.Read();
                        break;
                    case LexChoice.ReadAppend:
                        reader.Read();
                        if (state == LexCharType.WhileSpace)
                        {
                            state = nextCharType;
                        }
                        sb.Append(nextChar);
                        break;
                    case LexChoice.Terminate:
                        yield break;
                    default:
                        throw new Exception("Assert failed!");
                }

            }

        }
        private static TokenType GetTokenType(string word)
        {
            return TokenType.GetAll().FirstOrDefault(type => type.Match(word));
        }

        private static Token GetToken(string word)
        {
            TokenType tokenType = GetTokenType(word);
            return new Token() { Value = word, TokenType = tokenType };
        }

        private static LexCharType GetCharType(Char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                return LexCharType.WhileSpace;
            }
            else if (Char.IsLetter(ch) || Char.IsDigit(ch) || ch == '_')
            {
                return LexCharType.LetterOrDigitOrUnderscore;
            }
            else if ('!' <= ch && ch <= '/' || ':' <= ch && ch <= '@' || '[' <= ch && ch <= '`' && ch != '_' || '{' <= ch && ch <= '~')
            {
                return LexCharType.Punctuation;
            }
            else
            {
                return LexCharType.Unknown;
            }
        }
    }
}
