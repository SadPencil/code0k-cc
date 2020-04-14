using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace code0k_cc
{
    partial class Token
    {

        public readonly TokenType Type;
        /// <summary>
        /// 匹配的正则表达式
        /// </summary>
        private readonly string pattern;

        /// <summary>
        /// 友好的名称
        /// </summary>
        public string Name { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

        private Token(TokenType type, string pattern, string name)
        {
            this.Type = type;
            this.pattern = pattern;
            this.Name = name;
        }

        public bool Match(string str)
        {
            var pattern = @"^" + this.pattern + @"$";
            return Regex.IsMatch(str, pattern);
        }

        public static readonly Token Input = new Token(TokenType.Keyword, "input", "input");
        public static readonly Token Output = new Token(TokenType.Keyword, "output", "output");
        public static readonly Token NizkInput = new Token(TokenType.Keyword, "nizkinput", "nizkinput");
        public static readonly Token Const = new Token(TokenType.Keyword, "const", "const");
        public static readonly Token Var = new Token(TokenType.Keyword, "var", "var");

        //public static readonly Symbol Int = new Symbol(Type.Keyword, "int(32|64|128|256|512)", "int");
        //public static readonly Symbol UInt = new Symbol(Type.Keyword, "uint(32|64|128|256|512)", "uint");

        // todo: support fixed point number
        // public static readonly Symbol Fixed = new Symbol(Type.Keyword, "fixed(64|128|256)", "fixed");

        public static readonly Token Call = new Token(TokenType.Keyword, "call", "call");
        public static readonly Token Procedure = new Token(TokenType.Keyword, "procedure", "procedure");
        public static readonly Token Function = new Token(TokenType.Keyword, "function", "function");
        public static readonly Token Begin = new Token(TokenType.Keyword, "begin", "begin");
        public static readonly Token End = new Token(TokenType.Keyword, "end", "end");
        public static readonly Token Return = new Token(TokenType.Keyword, "return", "return");

        public static readonly Token If = new Token(TokenType.Keyword, "if", "if");
        public static readonly Token Then = new Token(TokenType.Keyword, "then", "then");

        //public static readonly Symbol Read = new Symbol(Type.Keyword, "read", "read");
        //public static readonly Symbol Write = new Symbol(Type.Keyword, "write", "write");

        public static readonly Token While = new Token(TokenType.Keyword, "while", "while");
        public static readonly Token Do = new Token(TokenType.Keyword, "do", "do");
        public static readonly Token Max = new Token(TokenType.Keyword, "max", "max");
        public static readonly Token For = new Token(TokenType.Keyword, "for", "for");
        public static readonly Token To = new Token(TokenType.Keyword, "to", "to");
        public static readonly Token DownTo = new Token(TokenType.Keyword, "downto", "downto");

        public static readonly Token LeftBracket = new Token(TokenType.NonLetterKeyword, "\\(", "(");
        public static readonly Token RightBracket = new Token(TokenType.NonLetterKeyword, "\\)", ")");

        public static readonly Token Comma = new Token(TokenType.NonLetterKeyword, "\\,", ",");
        public static readonly Token Semicolon = new Token(TokenType.NonLetterKeyword, "\\;", ";");
        public static readonly Token Assign = new Token(TokenType.NonLetterKeyword, "\\:\\=", ":=");

        public static readonly Token Plus = new Token(TokenType.NonLetterKeyword, "\\+", "+");
        public static readonly Token Minus = new Token(TokenType.NonLetterKeyword, "\\-", "-");
        public static readonly Token Times = new Token(TokenType.NonLetterKeyword, "\\*", "*");

        //todo: add support for division
        public static readonly Token Divide = new Token(TokenType.Keyword, "div", "div");
        public static readonly Token Mod = new Token(TokenType.Keyword, "mod", "mod");

        public static readonly Token EqualTo = new Token(TokenType.NonLetterKeyword, "(\\=|\\==)", "=");
        public static readonly Token LessThan = new Token(TokenType.NonLetterKeyword, "\\<", "<");
        public static readonly Token GreaterThan = new Token(TokenType.NonLetterKeyword, "\\>", ">");
        public static readonly Token LessEqualThan = new Token(TokenType.NonLetterKeyword, "\\<\\=", "<=");
        public static readonly Token GreaterEqualThan = new Token(TokenType.NonLetterKeyword, "\\>\\=", ">=");
        public static readonly Token NotEqualTo = new Token(TokenType.NonLetterKeyword, "(\\#|\\!\\=)", "#");

        //todo: add bitwise operation

        public static readonly Token Dot = new Token(TokenType.NonLetterKeyword, "\\.", ".");
        public static readonly Token Colon = new Token(TokenType.NonLetterKeyword, "\\:", ":");

        public static readonly Token Odd = new Token(TokenType.Keyword, "odd", "odd");

        public static readonly Token Identifier = new Token(TokenType.Identifier, "[_a-zA-z][_a-zA-z0-9]{0,200}", "identifier");

        public static readonly Token UnsignedNumber = new Token(TokenType.Number, "(0|[1-9][0-9]{0,200})u", "unsignednumber");
        public static readonly Token SignedNumber = new Token(TokenType.Number, "(0|[1-9][0-9]{0,200})", "signednumber");

        //todo: add hex number support
        //todo: add fixed number support


        public static List<Token> GetAll()
        {
            // note that the order matters
            return new List<Token>
            {
                Input, 
                Output,
                NizkInput,
                Const,
                Var,
                
                Call,
                Procedure,
                Function,
                Begin,
                End,
                Return,

                If,
                Then,
                               
                While,
                Do,
                Max,

                For,
                To,
                DownTo,

                LeftBracket,
                RightBracket,

                Comma,
                Semicolon,
                Assign,

                Plus,
                Minus,
                Times,

                Divide,
                Mod,

                EqualTo,
                LessThan,
                GreaterThan,
                LessEqualThan,
                GreaterEqualThan,
                NotEqualTo,

                Dot,
                Colon,

                Odd,

                //这俩必须依次排在最后
                UnsignedNumber,
                SignedNumber,
                Identifier,
            };
        }



    }


}
