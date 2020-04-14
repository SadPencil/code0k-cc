using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace code0k_cc
{
    class LexicalAnalyzer
    {


        public SymbolTable Analyze(String content)
        {
            SymbolTable table = new SymbolTable();
            StringWriter word = null;
            int contentPosition = -1;

            bool wordIsANumber = false;
            bool wordIsAnOperator = false;

            //读入一个词
            while (true)
            {
                if (++contentPosition >= content.Length)
                {
                    if (word != null)
                    {
                        table.Append(word.ToString());
                    }
                    break;
                }

                char nextChar = content[contentPosition];
                bool isVisible = !Char.IsWhiteSpace(nextChar);

                //System.out.println("char:"+nextChar);
                //System.out.println("byte:"+nextByte);
                //System.out.println("word:["+word+"]");

                if (word == null)
                {
                    if (isVisible)
                    {
                        word = new StringWriter();
                        word.Write(nextChar);

                        if (Char.IsDigit(nextChar))
                        {
                            wordIsAnOperator = false;
                            wordIsANumber = true;
                        }
                        else if (Char.IsLetter(nextChar))
                        {
                            wordIsAnOperator = false;
                            wordIsANumber = false;
                        }
                        else
                        {
                            wordIsAnOperator = true;
                            wordIsANumber = false;
                        }
                        //System.out.println("1+word:"+word);
                    }
                    else
                    {
                        //do nothing
                    }
                }
                else
                {
                    if (!isVisible)
                    {
                        table.Append(word.ToString());
                        //System.out.println("2-word:"+word);
                        word = null;
                    }
                    else
                    {
                        if (Char.IsDigit(nextChar))
                        {
                            if (wordIsAnOperator)
                            {
                                table.Append(word.ToString());
                                //System.out.println("3-word:"+word);
                                word = null;
                                //退回字符
                                --contentPosition;
                            }
                            else if (wordIsANumber)
                            {
                                word.Write(nextChar);
                                //System.out.println("4+word:"+word);
                            }
                            else
                            {
                                word.Write(nextChar);
                                //System.out.println("5+word:"+word);
                            }
                        }
                        else if (Char.IsLetter(nextChar))
                        {
                            if (wordIsAnOperator)
                            {
                                table.Append(word.ToString());
                                //System.out.println("6-word:"+word);

                                word = null;
                                //退回字符
                                --contentPosition;
                            }
                            else if (wordIsANumber)
                            {
                                table.Append(word.ToString());
                                //System.out.println("7-word:"+word);
                                word = null;
                                //退回字符
                                --contentPosition;
                            }
                            else
                            {
                                word.Write(nextChar);
                                //System.out.println("8+word:"+word);
                            }
                        }
                        else
                        {
                            if (wordIsAnOperator)
                            {
                                //两符号相遇，分两种情况

                                //如果当前能匹配
                                if (table.TryAppend(word.ToString()))
                                {
                                    //如果增加这个字符还能匹配，则先不匹配
                                    if (table.TryAppend(word.ToString() + nextChar))
                                    {
                                        word.Write(nextChar);
                                        //System.out.println("9.1+word:"+word);
                                    }
                                    else
                                    {
                                        table.Append(word.ToString());
                                        //System.out.println("9.2+word:"+word);
                                        word = null;
                                        //退回字符
                                        --contentPosition;
                                    }
                                }
                                else
                                {
                                    //不能匹配
                                    word.Write(nextChar);
                                    //System.out.println("9.3+word:"+word);
                                }

                            }
                            else if (wordIsANumber)
                            {
                                table.Append(word.ToString());
                                //System.out.println("10-word:"+word);
                                word = null;
                                //退回字符
                                --contentPosition;
                            }
                            else
                            {
                                table.Append(word.ToString());
                                //System.out.println("11-word:"+word);
                                word = null;
                                //退回字符
                                --contentPosition;
                            }
                        }
                    }
                }

            }

            return table;
        }
    }
}
