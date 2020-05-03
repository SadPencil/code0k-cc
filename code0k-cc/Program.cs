using System;
using System.Diagnostics;
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
            Debug.WriteLine(null);
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
            Debug.WriteLine($"root blk {blk}");
            var ret = mainProgram.Execute(new ExeArg() { Block = blk });


            //debug


            Debug.WriteLine(ret.ExpressionResult.VariableRefRef.VariableRef.Variable);
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
