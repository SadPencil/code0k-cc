using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace code0k_cc
{
    class TokenType
    {

        public readonly TokenTypeType Type;
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

        private TokenType(TokenTypeType type, string pattern, string name)
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

        public static readonly TokenType Input = new TokenType(TokenTypeType.Keyword, "input", "input");
        public static readonly TokenType Output = new TokenType(TokenTypeType.Keyword, "output", "output");
        public static readonly TokenType NizkInput = new TokenType(TokenTypeType.Keyword, "nizkinput", "nizkinput");
        public static readonly TokenType Const = new TokenType(TokenTypeType.Keyword, "const", "const");
        public static readonly TokenType Var = new TokenType(TokenTypeType.Keyword, "var", "var");
        public static readonly TokenType Ref = new TokenType(TokenTypeType.Keyword, "ref", "ref");
        //public static readonly Symbol Int = new Symbol(Type.Keyword, "int(32|64|128|256|512)", "int");
        //public static readonly Symbol UInt = new Symbol(Type.Keyword, "uint(32|64|128|256|512)", "uint");

        // todo: support fixed point number
        // public static readonly Symbol Fixed = new Symbol(Type.Keyword, "fixed(64|128|256)", "fixed");

        public static readonly TokenType Call = new TokenType(TokenTypeType.Keyword, "call", "call");
        public static readonly TokenType Procedure = new TokenType(TokenTypeType.Keyword, "procedure", "procedure");
        public static readonly TokenType Function = new TokenType(TokenTypeType.Keyword, "function", "function");
        public static readonly TokenType Begin = new TokenType(TokenTypeType.NonLetterKeyword, "{", "{");
        public static readonly TokenType End = new TokenType(TokenTypeType.NonLetterKeyword, "}", "}");
        public static readonly TokenType Return = new TokenType(TokenTypeType.Keyword, "return", "return");

        public static readonly TokenType If = new TokenType(TokenTypeType.Keyword, "if", "if");
        public static readonly TokenType Then = new TokenType(TokenTypeType.Keyword, "then", "then");
        public static readonly TokenType Else = new TokenType(TokenTypeType.Keyword, "else", "else");

        //public static readonly Symbol Read = new Symbol(Type.Keyword, "read", "read");
        //public static readonly Symbol Write = new Symbol(Type.Keyword, "write", "write");

        public static readonly TokenType While = new TokenType(TokenTypeType.Keyword, "while", "while");
        public static readonly TokenType Do = new TokenType(TokenTypeType.Keyword, "do", "do");
        public static readonly TokenType Max = new TokenType(TokenTypeType.Keyword, "max", "max");
        public static readonly TokenType For = new TokenType(TokenTypeType.Keyword, "for", "for");
        public static readonly TokenType To = new TokenType(TokenTypeType.Keyword, "to", "to");
        public static readonly TokenType DownTo = new TokenType(TokenTypeType.Keyword, "downto", "downto");

        public static readonly TokenType Break = new TokenType(TokenTypeType.Keyword, "break", "break");
        public static readonly TokenType Continue = new TokenType(TokenTypeType.Keyword, "continue", "continue");

        public static readonly TokenType LeftBracket = new TokenType(TokenTypeType.NonLetterKeyword, "\\(", "(");
        public static readonly TokenType RightBracket = new TokenType(TokenTypeType.NonLetterKeyword, "\\)", ")");
        public static readonly TokenType LeftSquareBracket = new TokenType(TokenTypeType.NonLetterKeyword, "\\[", "[");
        public static readonly TokenType RightSquareBracket = new TokenType(TokenTypeType.NonLetterKeyword, "\\]", "]");

        public static readonly TokenType Comma = new TokenType(TokenTypeType.NonLetterKeyword, "\\,", ",");
        public static readonly TokenType Semicolon = new TokenType(TokenTypeType.NonLetterKeyword, "\\;", ";");
        public static readonly TokenType Assign = new TokenType(TokenTypeType.NonLetterKeyword, "\\:\\=", ":=");

        public static readonly TokenType Plus = new TokenType(TokenTypeType.NonLetterKeyword, "\\+", "+");
        public static readonly TokenType Minus = new TokenType(TokenTypeType.NonLetterKeyword, "\\-", "-");
        public static readonly TokenType Times = new TokenType(TokenTypeType.NonLetterKeyword, "\\*", "*");

        //todo: add support for division
        public static readonly TokenType Divide = new TokenType(TokenTypeType.Keyword, "div", "div");
        public static readonly TokenType Mod = new TokenType(TokenTypeType.Keyword, "mod", "mod");

        public static readonly TokenType EqualTo = new TokenType(TokenTypeType.NonLetterKeyword, "(\\=|\\==)", "=");
        public static readonly TokenType LessThan = new TokenType(TokenTypeType.NonLetterKeyword, "\\<", "<");
        public static readonly TokenType GreaterThan = new TokenType(TokenTypeType.NonLetterKeyword, "\\>", ">");
        public static readonly TokenType LessEqualThan = new TokenType(TokenTypeType.NonLetterKeyword, "\\<\\=", "<=");
        public static readonly TokenType GreaterEqualThan = new TokenType(TokenTypeType.NonLetterKeyword, "\\>\\=", ">=");
        public static readonly TokenType NotEqualTo = new TokenType(TokenTypeType.NonLetterKeyword, "(\\#|\\!\\=)", "#");

        //todo: add bitwise operation

        public static readonly TokenType Dot = new TokenType(TokenTypeType.NonLetterKeyword, "\\.", ".");
        public static readonly TokenType Colon = new TokenType(TokenTypeType.NonLetterKeyword, "\\:", ":");

        public static readonly TokenType Odd = new TokenType(TokenTypeType.Keyword, "odd", "odd");

        public static readonly TokenType Identifier = new TokenType(TokenTypeType.Identifier, "[_a-zA-z][_a-zA-z0-9]{0,200}", "identifier");

        public static readonly TokenType Number = new TokenType(TokenTypeType.Number, "([0-9][0-9_a-zA-z\\.]{0,200}", "number");

        //todo: add hex number support
        //todo: add fixed number support


        public static IEnumerable<TokenType> GetAll()
        {
            yield return Input;
            yield return Output;
            yield return NizkInput;
            yield return Const;
            yield return Var;
            yield return Ref;

            yield return Call;
            yield return Procedure;
            yield return Function;
            yield return Begin;
            yield return End;
            yield return Return;

            yield return If;
            yield return Then;
            yield return Else;

            yield return While;
            yield return Do;
            yield return Max;

            yield return For;
            yield return To;
            yield return DownTo;

            yield return Break;
            yield return Continue;

            yield return LeftBracket;
            yield return RightBracket;

            yield return Comma;
            yield return Semicolon;
            yield return Assign;

            yield return Plus;
            yield return Minus;
            yield return Times;

            yield return Divide;
            yield return Mod;

            yield return EqualTo;
            yield return LessThan;
            yield return GreaterThan;
            yield return LessEqualThan;
            yield return GreaterEqualThan;
            yield return NotEqualTo;

            yield return Dot;
            yield return Colon;

            yield return Odd;

            // note that the order matters
            yield return UnsignedNumber;
            yield return Number;
            yield return Identifier;
        }



    }


}
