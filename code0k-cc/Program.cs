using System;
using System.IO;

namespace code0k_cc
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var fs = File.OpenRead("1.txt"))
            {
                var tokens = Lex.Analyze(fs);
                var ret = Parser.Parse(tokens);
            }

        }
    }
}
