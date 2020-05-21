using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using code0k_cc.Parse;
using code0k_cc.Pinocchio;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.ExeArg;

namespace code0k_cc
{
    class Program
    {
        private static void ShowHelp()
        {
            Process cur = Process.GetCurrentProcess();
            Console.WriteLine("Usage: ");
            Console.WriteLine($"\t{ ( cur.ProcessName.Contains('\"', StringComparison.InvariantCulture) ? ( "\"" + cur.ProcessName + "\"" ) : cur.ProcessName )} <input-file>");
        }
        static void Main(string[] args)
        {
#if DEBUG
            var path = "input.txt";
#else
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var path = args[0];
#endif


            Debug.WriteLine(path);

            ParseUnitInstance mainProgram;
            using (var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var tokens = Lex.Lex.Analyze(fs);
                var tokenList = tokens.ToList();
                ////test
                //foreach (var token in tokenList)
                //{
                //    Console.WriteLine(token.ToString());
                //}

                mainProgram = Parser.Parse(tokenList);
            }

            string outPath = path + ".arith";
            if (File.Exists(outPath))
            {
                File.Delete(outPath);
            }

            using (var fs = System.IO.File.Open(outPath, FileMode.CreateNew, FileAccess.Write))
            {
                using (var outputWriter = new StreamWriter(fs, new UTF8Encoding(false), -1, false))
                {
                    OverlayBlock blk = new OverlayBlock(new Overlay(null), new BasicBlock(null));
                    Debug.WriteLine($"root blk {blk}");
                    var ret = mainProgram.Execute(
                        new ExeArg(
                            blk,
                            new CallStack(null, null),
                            outputWriter,
                            Console.Out));
                    var map = ret.MainProgramResult.VariableMap;
                    var pinocchio = new PinocchioArithmeticCircuit(map);

                    pinocchio.OutputCircuit(outputWriter);
                }
            }
        }

        //static void test(ParseUnitInstance p, int tab)
        //{
        //    for (int i = 0; i < tab; ++i)
        //        Console.Write(" ");
        //    if (p == null) { Console.WriteLine("NULL"); }
        //    else
        //    {
        //        Console.WriteLine(p.ParseUnit.Name);
        //        if (p.Children != null)
        //            foreach (var pp in p.Children)
        //            {
        //                test(pp, tab + 1);
        //            }
        //    }
        //}
    }
}
