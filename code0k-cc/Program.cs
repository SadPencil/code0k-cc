using System;
using System.IO;
using System.Linq;
using code0k_cc.Parse;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.ExeArg;

namespace code0k_cc
{
    class Program
    {
        static void Main(string[] args)
        {
            ParseInstance mainProgram;
            using (var fs = File.OpenRead("1.txt"))
            {
                var tokens = Lex.Lex.Analyze(fs);
                var tokenList = tokens.ToList();
                //test
                foreach (var token in tokenList)
                {
                    Console.WriteLine(token.ToString());
                }


                mainProgram = Parser.Parse(tokenList);
            }
            //   var ret = Parser.Parse(tokens);
            test(mainProgram, 0);


            OverlayBlock blk = new OverlayBlock(new Overlay(null), new BasicBlock(null));
            mainProgram.Execute(new ExeArg() { Block = blk });


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
