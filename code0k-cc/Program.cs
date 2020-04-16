using System;
using System.IO;
using System.Linq;

namespace code0k_cc
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var fs = File.OpenRead("1.txt"))
            {
                var tokens = Lex.Analyze(fs);
                var tokenList = tokens.ToList();
                //test
                foreach (var token in tokenList)
                {
                    Console.WriteLine(token.ToString());
                }


                var ret = Parser.Parse(tokenList);
                //   var ret = Parser.Parse(tokens);
                test(ret, 0);
            }

            //debug



        }

        static void test(ParseInstance p, int tab)
        {
            for (int i = 0; i < tab; ++i)
                Console.Write(" ");
            if (p == null) { Console.WriteLine("NULL"); }
            else
            {
                Console.WriteLine(p.ParseUnit.Name);
                if (p.Children != null)
                    foreach (var pp in p.Children)
                    {
                        test(pp, tab + 1);
                    }
            }
        }
    }
}
