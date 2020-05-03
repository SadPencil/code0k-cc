using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace code0k_cc.Lex
{
    class TokenType
    {

        private readonly string Pattern;
        public readonly string Name;

        private TokenType() { }

        public override string ToString()
        {
            return this.Name;
        }

        private TokenType(string pattern, string name)
        {
            this.Pattern = pattern;
            this.Name = name;
        }

        public bool Match(string str)
        {
            var pattern = @"^" + this.Pattern + @"$";
            return Regex.IsMatch(str, pattern);
        }

        public static readonly TokenType EOL = new TokenType("\\b", "EOL");
        public static readonly TokenType Print = new TokenType("print", "print");

        public static readonly TokenType Input = new TokenType("input", "input");
        public static readonly TokenType Output = new TokenType("output", "output");
        public static readonly TokenType NizkInput = new TokenType("nizkinput", "nizkinput");
        //public static readonly TokenType Const = new TokenType("const", "const");
        //public static readonly TokenType Var = new TokenType("var", "var");
        //public static readonly TokenType Ref = new TokenType("ref", "ref");

        //public static readonly TokenType Call = new TokenType("call", "call");

        public static readonly TokenType If = new TokenType("if", "if");
        public static readonly TokenType Then = new TokenType("then", "then");
        public static readonly TokenType Else = new TokenType("else", "else");

        public static readonly TokenType While = new TokenType("while", "while");
        public static readonly TokenType Max = new TokenType("max", "max");

        //public static readonly TokenType Do = new TokenType("do", "do");
        //public static readonly TokenType For = new TokenType("for", "for");
        //public static readonly TokenType To = new TokenType("to", "to");
        //public static readonly TokenType DownTo = new TokenType("downto", "downto");

        public static readonly TokenType Break = new TokenType("break", "break");
        public static readonly TokenType Continue = new TokenType("continue", "continue");
        public static readonly TokenType Return = new TokenType("return", "return");

        public static readonly TokenType True = new TokenType("true", "true");
        public static readonly TokenType False = new TokenType("false", "false");

        public static readonly TokenType Begin = new TokenType("{", "{");
        public static readonly TokenType End = new TokenType("}", "}");
        public static readonly TokenType LeftBracket = new TokenType("\\(", "(");
        public static readonly TokenType RightBracket = new TokenType("\\)", ")");
        public static readonly TokenType LeftSquareBracket = new TokenType("\\[", "[");
        public static readonly TokenType RightSquareBracket = new TokenType("\\]", "]");

        public static readonly TokenType Dot = new TokenType("\\.", ".");
        public static readonly TokenType Colon = new TokenType("\\:", ":");
        public static readonly TokenType Comma = new TokenType("\\,", ",");
        public static readonly TokenType Semicolon = new TokenType("\\;", ";");
        public static readonly TokenType Assign = new TokenType("\\=", "=");

        public static readonly TokenType Plus = new TokenType("\\+", "+");
        public static readonly TokenType Minus = new TokenType("\\-", "-");
        public static readonly TokenType Times = new TokenType("\\*", "*");
        public static readonly TokenType Divide = new TokenType("\\/", "/");
        public static readonly TokenType Mod = new TokenType("\\%", "%");

        public static readonly TokenType EqualTo = new TokenType("\\=\\=", "==");
        public static readonly TokenType LessThan = new TokenType("\\<", "<");
        public static readonly TokenType GreaterThan = new TokenType("\\>", ">");
        public static readonly TokenType LessEqualThan = new TokenType("\\<\\=", "<=");
        public static readonly TokenType GreaterEqualThan = new TokenType("\\>\\=", ">=");
        public static readonly TokenType NotEqualTo = new TokenType("\\!\\=", "!=");

        public static readonly TokenType BitwiseAnd = new TokenType("\\&", "&");
        public static readonly TokenType BitwiseOr = new TokenType("\\|", "|");
        public static readonly TokenType BitwiseXor = new TokenType("\\^", "^");
        public static readonly TokenType BitwiseNot = new TokenType("\\~", "~" );

        public static readonly TokenType BooleanAnd = new TokenType("\\&\\&", "&&");
        public static readonly TokenType BooleanOr = new TokenType("\\|\\|", "||");
        public static readonly TokenType BooleanXor = new TokenType("\\^\\^", "^^");
        public static readonly TokenType BooleanNot = new TokenType("\\!", "!" );

        public static readonly TokenType BitwiseLeftShiftSigned = new TokenType("\\<\\<", "<<");
        public static readonly TokenType BitwiseLeftShiftUnsigned = new TokenType("\\<\\<\\<", "<<<");
        public static readonly TokenType BitwiseRightShiftSigned = new TokenType("\\>\\>", ">>");
        public static readonly TokenType BitwiseRightShiftUnsigned = new TokenType("\\>\\>\\>", ">>>");

        public static readonly TokenType String = new TokenType("\\\"(\\\\.|[^\\\"])*\\\"", "string");

        public static readonly TokenType Identifier = new TokenType("[_a-zA-z][_a-zA-z0-9]{0,200}", "identifier");
        public static readonly TokenType Number = new TokenType("[0-9][0-9_a-zA-z\\.]{0,200}", "number");

        public static IEnumerable<TokenType> GetAll()
        {
            // EOL not included

            yield return Print;

            yield return Input;
            yield return NizkInput;
            yield return Output;

            yield return If;
            yield return Then;
            yield return Else;

            yield return While;
            yield return Max;

            yield return Break;
            yield return Continue;
            yield return Return;

            yield return True;
            yield return False;

            yield return Begin;
            yield return End;
            yield return LeftBracket;
            yield return RightBracket;
            yield return LeftSquareBracket;
            yield return RightSquareBracket;

            yield return Dot;
            yield return Colon;
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

            yield return BitwiseAnd;
            yield return BitwiseOr;
            yield return BitwiseXor;
            yield return BitwiseNot;

            yield return BooleanAnd;
            yield return BooleanOr;
            yield return BooleanXor;
            yield return BooleanNot;

            yield return BitwiseLeftShiftSigned;
            yield return BitwiseRightShiftSigned;
            yield return BitwiseLeftShiftUnsigned;
            yield return BitwiseRightShiftUnsigned;

            yield return String;
            // note that the order matters 
            yield return Number;
            yield return Identifier;
        }



    }


}
