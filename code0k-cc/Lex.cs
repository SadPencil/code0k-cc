using System;
using System.Collections.Generic;
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
            StringBuilder word = new StringBuilder();

            while (true)
            {
                Int32 nextCharInt = reader.Peek();
                Console.WriteLine(nextCharInt);
                Console.WriteLine((char) nextCharInt);
                if (nextCharInt == -1) // EOL
                {
                    if (word.Length > 0)
                    {
                        yield return GetToken(word.ToString());
                    }
                    yield break;
                }

                char nextChar = (char)nextCharInt;

                var nextCharType = (
                    IsWhiteSpace: Char.IsWhiteSpace(nextChar),
                    IsLetterOrDigitOrUnderscore: Char.IsLetter(nextChar) || Char.IsDigit(nextChar) || nextChar == '_'
                );

                // ugly because C# doesn't support inline enum
                var choiceEnum = (ReadAppend: false, ReadReturn: false, PeekReturn: false);

                if (word.Length == 0)
                {
                    if (!nextCharType.IsWhiteSpace)
                    {
                        choiceEnum.ReadAppend = true;
                    }
                }
                else
                {
                    if (nextCharType.IsWhiteSpace)
                    {
                        choiceEnum.ReadReturn = true;
                    }
                    else
                    {
                        if (nextCharType.IsLetterOrDigitOrUnderscore)
                        {
                            if ((!nextCharType.IsLetterOrDigitOrUnderscore))
                            {
                                choiceEnum.PeekReturn = true;
                            }
                            else
                            {
                                choiceEnum.ReadAppend = true;
                            }
                        }
                        else
                        {
                            if ((!nextCharType.IsLetterOrDigitOrUnderscore))
                            {
                                // consider '&' and '&&'
                                if (GetTokenType(word.ToString()) != null)
                                {
                                    if (GetTokenType(word.ToString() + nextChar) != null)
                                    {
                                        choiceEnum.ReadAppend = true;
                                    }
                                    else
                                    {
                                        choiceEnum.PeekReturn = true;
                                    }
                                }
                                else
                                {
                                    choiceEnum.ReadAppend = true;
                                }
                            }
                            else
                            {
                                choiceEnum.PeekReturn = true;
                            }

                        }
                    }

                }

                if (choiceEnum.PeekReturn)
                {
                    yield return GetToken(word.ToString());
                    word.Clear();
                }
                else if (choiceEnum.ReadReturn)
                {
                    reader.Read();
                    yield return GetToken(word.ToString());
                    word.Clear();
                }
                else if (choiceEnum.ReadAppend)
                {
                    reader.Read();
                    word.Append(nextChar);
                }
                else
                {
                    throw new Exception("Assert failed!");
                }

            }

        }
        private static TokenType GetTokenType(string word)
        {
            return TokenType.GetAll().FirstOrDefault(type => { return type.Match(word); });

            //foreach (var symbol in TokenType.GetAll())
            //{
            //    if (symbol.Match(word))
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        private static Token GetToken(string word)
        {
            TokenType tokenType = GetTokenType(word);
            return new Token() { Value = word, TokenType = tokenType };
        }
    }
}
