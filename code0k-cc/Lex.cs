using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace code0k_cc
{
    static class Lex
    {
        public struct Token
        {
            public string token;
            public String Value;
        }

        static IEnumerable<Token> Next(Stream stream)
        {
            return Next(stream, new UTF8Encoding(false));
        }
        static IEnumerable<Token> Next(Stream stream, Encoding encoding)
        {
            StreamReader reader = new StreamReader(stream, encoding);

            Int32 nextChar = reader.Read();
            if (nextChar == -1) yield break; // EOL
            while (true)
            {
                StringBuilder word = new StringBuilder();
            }

        }
    }
}
