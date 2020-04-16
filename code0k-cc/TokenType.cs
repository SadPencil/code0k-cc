using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace code0k_cc
{
    class TokenType
    {

        public readonly TokenTypeProperty Property;
        /// <summary>
        /// 匹配的正则表达式
        /// </summary>
        private readonly string Pattern;

        /// <summary>
        /// 友好的名称
        /// </summary>
        public string Name { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

        private TokenType(string pattern, string name) : this(pattern, name, new TokenTypeProperty()) { }
        private TokenType(string pattern, string name, TokenTypeProperty property)
        {
            if (property == null)
            {
                property = new TokenTypeProperty();
            }
            this.Property = property;
            this.Pattern = pattern;
            this.Name = name;
        }

        public bool Match(string str)
        {
            var pattern = @"^" + this.Pattern + @"$";
            return Regex.IsMatch(str, pattern);
        }

        public static readonly TokenType Input = new TokenType("input", "input", new TokenTypeProperty() { IsDescriptionWord = true });
        public static readonly TokenType Output = new TokenType("output", "output", new TokenTypeProperty() { IsDescriptionWord = true });
        public static readonly TokenType NizkInput = new TokenType("nizkinput", "nizkinput", new TokenTypeProperty() { IsDescriptionWord = true });
        public static readonly TokenType Const = new TokenType("const", "const", new TokenTypeProperty() { IsDescriptionWord = true });
        public static readonly TokenType Var = new TokenType("var", "var", new TokenTypeProperty() { IsDescriptionWord = true });
        public static readonly TokenType Ref = new TokenType("ref", "ref", new TokenTypeProperty() { IsDescriptionWord = true });

        public static readonly TokenType Call = new TokenType("call", "call");
        public static readonly TokenType Return = new TokenType("return", "return");

        public static readonly TokenType If = new TokenType("if", "if");
        public static readonly TokenType Then = new TokenType("then", "then");
        public static readonly TokenType Else = new TokenType("else", "else");

        //public static readonly Symbol Read = new Symbol(Type.Keyword, "read", "read");
        //public static readonly Symbol Write = new Symbol(Type.Keyword, "write", "write");

        public static readonly TokenType While = new TokenType("while", "while");
        public static readonly TokenType Do = new TokenType("do", "do");
        public static readonly TokenType Max = new TokenType("max", "max");
        public static readonly TokenType For = new TokenType("for", "for");
        public static readonly TokenType To = new TokenType("to", "to");
        public static readonly TokenType DownTo = new TokenType("downto", "downto");

        public static readonly TokenType Break = new TokenType("break", "break");
        public static readonly TokenType Continue = new TokenType("continue", "continue");

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

        public static readonly TokenType Plus = new TokenType("\\+", "+", new TokenTypeProperty() { IsUnaryOperator = true, IsBinaryOperator = true });
        public static readonly TokenType Minus = new TokenType("\\-", "-", new TokenTypeProperty() { IsUnaryOperator = true, IsBinaryOperator = true });
        public static readonly TokenType Times = new TokenType("\\*", "*", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType Divide = new TokenType("\\\\", "\\", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType Mod = new TokenType("\\%", "%", new TokenTypeProperty() { IsBinaryOperator = true });

        public static readonly TokenType EqualTo = new TokenType("\\=\\=", "==", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType LessThan = new TokenType("\\<", "<", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType GreaterThan = new TokenType("\\>", ">", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType LessEqualThan = new TokenType("\\<\\=", "<=", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType GreaterEqualThan = new TokenType("\\>\\=", ">=", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType NotEqualTo = new TokenType("\\!\\=", "!=", new TokenTypeProperty() { IsBinaryOperator = true });

        public static readonly TokenType BitwiseAnd = new TokenType("\\&", "&", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseOr = new TokenType("\\|", "|", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseXor = new TokenType("\\^", "^", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseNot = new TokenType("\\~", "~", new TokenTypeProperty() { IsUnaryOperator = true });

        public static readonly TokenType BooleanAnd = new TokenType("\\&\\&", "&&", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BooleanOr = new TokenType("\\|\\|", "||", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BooleanXor = new TokenType("\\^\\^", "^^", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BooleanNot = new TokenType("\\!", "!", new TokenTypeProperty() { IsUnaryOperator = true });

        public static readonly TokenType BitwiseLeftShiftSigned = new TokenType("\\<\\<", "<<", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseLeftShiftUnsigned = new TokenType("\\<\\<\\<", "<<<", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseRightShiftSigned = new TokenType("\\>\\>", ">>", new TokenTypeProperty() { IsBinaryOperator = true });
        public static readonly TokenType BitwiseRightShiftUnsigned = new TokenType("\\>\\>\\>", ">>>", new TokenTypeProperty() { IsBinaryOperator = true });

        public static readonly TokenType Identifier = new TokenType("[_a-zA-z][_a-zA-z0-9]{0,200}", "identifier");
        public static readonly TokenType Number = new TokenType("[0-9][0-9_a-zA-z\\.]{0,200}", "number");


        public static IEnumerable<TokenType> GetAll()
        {
            yield return Input;
            yield return Output;
            yield return NizkInput;
            yield return Const;
            yield return Var;
            yield return Ref;

            yield return Call;
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

            // note that the order matters 
            yield return Number;
            yield return Identifier;
        }



    }


}
