using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace code0k_cc
{
    class Parser
    {
        private static ParseUnit ParseUnit { get; } = GetRootParseUnit();


        private static ParseUnit GetRootParseUnit()
        {
            var TokenUnits = new Dictionary<TokenType, ParseUnit>();
            foreach (var tokenType in TokenType.GetAll())
            {
                TokenUnits.Add(tokenType, new ParseUnit()
                {
                    Name = "Token " + tokenType.Name,
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.LeafNode,
                    LeafNodeTokenType = tokenType
                });
            }

            ParseUnit NeverMatchUnit = new ParseUnit()
            {
                Name = "Never Match Unit",
                Type = ParseUnitType.Single,
                ChildType = ParseUnitChildType.FirstChild,
                Children = new List<ParseUnit>() { }
            };

            ParseUnit NullMatchUnit = new ParseUnit()
            {
                Name = "Null Match Unit",
                Type = ParseUnitType.Single,
                ChildType = ParseUnitChildType.AllChild,
                Children = new List<ParseUnit>() { }
            };

            ParseUnit MainProgram = new ParseUnit();
            ParseUnit FunctionDeclaration = new ParseUnit();
            ParseUnit FunctionImplementation = new ParseUnit();

            ParseUnit FunctionDeclarationArguments = new ParseUnit();
            ParseUnit FunctionArgumentUnit = new ParseUnit();
            ParseUnit FunctionArgumentLastUnit = new ParseUnit();


            ParseUnit StatementBody = new ParseUnit();
            ParseUnit Statement = new ParseUnit();

            ParseUnit Descriptions = new ParseUnit();
            ParseUnit AssignStatement = new ParseUnit();

            ParseUnit DefinitionStatement = new ParseUnit();
            ParseUnit CallStatement = new ParseUnit();
            ParseUnit IfStatement = new ParseUnit();
            ParseUnit OptionalElseStatement = new ParseUnit();
            ParseUnit ForStatement = new ParseUnit();
            ParseUnit WhileStatement = new ParseUnit();
            ParseUnit CompoundStatement = new ParseUnit();

            ParseUnit LeftValue = new ParseUnit();
            ParseUnit RightValue = new ParseUnit();

            ParseUnit FunctionCallSuffix = new ParseUnit();
            ParseUnit ArraySubscripting = new ParseUnit();
            ParseUnit MemberAccess = new ParseUnit();

            ParseUnit Expression = new ParseUnit();

            const int OPERATOR_PRECEDENCE_LEVEL = 18;
            ParseUnit[] Expressions = new ParseUnit[OPERATOR_PRECEDENCE_LEVEL];
            ParseUnit[] Operators = new ParseUnit[OPERATOR_PRECEDENCE_LEVEL];

            foreach (var i in Enumerable.Range(0, OPERATOR_PRECEDENCE_LEVEL))
            {
                Expressions[i] = new ParseUnit();
                Operators[i] = new ParseUnit();
            }
            // no operator for level 0. this item should never be used
            Operators[0] = null;

            // write the parse unit
            MainProgram.Name = "Main Program";
            MainProgram.Type = ParseUnitType.Multiple;
            MainProgram.ChildType = ParseUnitChildType.FirstChild;
            MainProgram.Children = new List<ParseUnit>()
            {
                FunctionImplementation,
                FunctionDeclaration,
                DefinitionStatement
            };

            FunctionDeclaration.Name = "Function Declaration";
            FunctionDeclaration.Type = ParseUnitType.Single;
            FunctionImplementation.ChildType = ParseUnitChildType.AllChild;
            FunctionImplementation.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionDeclarationArguments,
                TokenUnits[TokenType.RightBracket]
            };

            FunctionImplementation.Name = "Function Implementation";
            FunctionImplementation.Type = ParseUnitType.Single;
            FunctionImplementation.ChildType = ParseUnitChildType.AllChild;
            FunctionImplementation.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionDeclarationArguments,
                TokenUnits[TokenType.RightBracket],
                CompoundStatement
            };

            FunctionDeclarationArguments.Name = "Function Arguments";
            FunctionDeclarationArguments.Type = ParseUnitType.SingleOptional;
            FunctionDeclarationArguments.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArguments.Children = new List<ParseUnit>()
            {
                FunctionArgumentUnit,
                FunctionArgumentLastUnit
            };

            FunctionArgumentUnit.Name = "Function Argument Unit";
            FunctionArgumentUnit.Type = ParseUnitType.MultipleOptional;
            FunctionArgumentUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Comma]
            };

            FunctionArgumentLastUnit.Name = "Function Argument Unit";
            FunctionArgumentLastUnit.Type = ParseUnitType.Single;
            FunctionArgumentLastUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentLastUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier]
            };

            FunctionCallSuffix.Name = "Function Call Suffix";
            FunctionCallSuffix.Type = ParseUnitType.Single;
            FunctionCallSuffix.ChildType = ParseUnitChildType.AllChild;
            FunctionCallSuffix.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                new ParseUnit()
                {
                    Name = "Function Call Argument",
                    Type = ParseUnitType.SingleOptional,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        new ParseUnit()
                        {
                            Name = "Function Call Argument Unit",
                            Type = ParseUnitType.MultipleOptional,
                            ChildType = ParseUnitChildType.AllChild,
                            Children = new List<ParseUnit>()
                            {
                                RightValue,
                                TokenUnits[TokenType.Comma]
                            }
                        },
                        new ParseUnit()
                        {
                            Name = "Function Call Argument Unit",
                            Type = ParseUnitType.Single,
                            ChildType = ParseUnitChildType.AllChild,
                            Children =  new List<ParseUnit>()
                            {
                                RightValue
                            }
                        }
                    }
                },
                TokenUnits[TokenType.RightBracket]
            };

            StatementBody.Name = "Statement Body";
            StatementBody.Type = ParseUnitType.MultipleOptional;
            StatementBody.ChildType = ParseUnitChildType.AllChild;
            StatementBody.Children = new List<ParseUnit>()
            {
                Statement,
                TokenUnits[TokenType.Semicolon]
            };

            Statement.Name = "Statement";
            Statement.Type = ParseUnitType.SingleOptional;
            Statement.ChildType = ParseUnitChildType.FirstChild;
            Statement.Children = new List<ParseUnit>()
            {
                DefinitionStatement,
                AssignStatement,
                CallStatement,
                IfStatement,
                ForStatement,
                WhileStatement,
                CompoundStatement
            };

            DefinitionStatement.Name = "Definition Statement";
            DefinitionStatement.Type = ParseUnitType.Single;
            DefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            DefinitionStatement.Children = new List<ParseUnit>()
            {
                Descriptions,
                TokenUnits[TokenType.Identifier],
                AssignStatement
            };

            Descriptions.Name = "Definition Description";
            Descriptions.Type = ParseUnitType.MultipleOptional;
            Descriptions.ChildType = ParseUnitChildType.FirstChild;
            Descriptions.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Input],
                TokenUnits[TokenType.NizkInput],
                TokenUnits[TokenType.Output],
                TokenUnits[TokenType.Const],
                TokenUnits[TokenType.Ref],
            };

            CallStatement.Name = "Call Statement";
            CallStatement.Type = ParseUnitType.Single;
            CallStatement.ChildType = ParseUnitChildType.AllChild;
            CallStatement.Children = new List<ParseUnit>()
            {
                FunctionCall
            };

            IfStatement.Name = "If Statement";
            IfStatement.Type = ParseUnitType.Single;
            IfStatement.ChildType = ParseUnitChildType.AllChild;
            IfStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.If],
                RightValue,
                TokenUnits[TokenType.Then],
                CompoundStatement,
                OptionalElseStatement
            };

            OptionalElseStatement.Name = "Else Statement";
            OptionalElseStatement.Type = ParseUnitType.SingleOptional;
            OptionalElseStatement.ChildType = ParseUnitChildType.AllChild;
            OptionalElseStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Else],
                CompoundStatement
            };

            ForStatement.Name = "For Statement";
            ForStatement.Type = ParseUnitType.Single;
            ForStatement.ChildType = ParseUnitChildType.AllChild;
            ForStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.For],
                LeftValue,
                TokenUnits[TokenType.Assign],
                RightValue,
                TokenUnits[TokenType.To],
                RightValue,
                TokenUnits[TokenType.Do],
                CompoundStatement
            };

            WhileStatement.Name = "While Statement";
            WhileStatement.Type = ParseUnitType.Single;
            WhileStatement.ChildType = ParseUnitChildType.AllChild;
            WhileStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.While],
                RightValue,
                TokenUnits[TokenType.Max],
                RightValue,
                TokenUnits[TokenType.Do],
                CompoundStatement
            };

            CompoundStatement.Name = "Compound Statement";
            CompoundStatement.Type = ParseUnitType.Single;
            CompoundStatement.ChildType = ParseUnitChildType.AllChild;
            CompoundStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End],
            };

            LeftValue.Name = "Left Value";
            LeftValue.Type = ParseUnitType.Single;
            LeftValue.ChildType = ParseUnitChildType.AllChild;
            LeftValue.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                new ParseUnit()
                {
                    Name="Left Value",
                    Type = ParseUnitType.MultipleOptional,
                    ChildType = ParseUnitChildType.FirstChild,
                    Children =  new List<ParseUnit>()
                    {
                        MemberAccess,
                        ArraySubscripting
                    }
                }
            };

            ArraySubscripting.Name = "Array Subscripting";
            ArraySubscripting.Type = ParseUnitType.Single;
            ArraySubscripting.ChildType = ParseUnitChildType.AllChild;
            ArraySubscripting.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftSquareBracket],
                RightValue,
                TokenUnits[TokenType.RightSquareBracket]
            };

            MemberAccess.Name = "Member Access";
            MemberAccess.Type = ParseUnitType.Single;
            MemberAccess.ChildType = ParseUnitChildType.AllChild;
            MemberAccess.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Dot],
                TokenUnits[TokenType.Identifier]
            };

            AssignStatement.Name = "Assign Statement";
            AssignStatement.Type = ParseUnitType.Single;
            AssignStatement.ChildType = ParseUnitChildType.AllChild;
            AssignStatement.Children = new List<ParseUnit>()
            {
                LeftValue,
                TokenUnits[TokenType.Assign],
                RightValue
            };

            RightValue.Name = "Right Value";
            RightValue.Type = ParseUnitType.Single;
            RightValue.ChildType = ParseUnitChildType.FirstChild;
            RightValue.Children = new List<ParseUnit>()
            {
                Expression
            };

            // the expression part is written according to the following precedence
            // https://en.cppreference.com/w/c/language/operator_precedence
            // Accessed at 2020-04-16

            // so I will follow the number from 1 to 17
            // although some of the operators are not implemented by now
            // these number are still reserved
            // Expression[0] means the identifier or number

            Expression.Name = "Expression";
            Expression.Type = ParseUnitType.Single;
            Expression.ChildType = ParseUnitChildType.FirstChild;
            Expression.Children = new List<ParseUnit>()
            {
                Expressions[OPERATOR_PRECEDENCE_LEVEL - 1]
            };

            foreach (var i in Enumerable.Range(0, OPERATOR_PRECEDENCE_LEVEL))
            {
                Expressions[i].Name = "Expression Level " + i.ToString(CultureInfo.InvariantCulture);
                Expressions[i].Type = ParseUnitType.Single;
                Expressions[i].ChildType = ParseUnitChildType.FirstChild;
            }


            // most of them are associated left-to-right
            // except for level 3 and level 16, which are right-to-left

            // level 0: Identifier or number
            Expressions[0].Children = new List<ParseUnit>() { TokenUnits[TokenType.Identifier], TokenUnits[TokenType.Number] };
            // level 1: not used
            Expressions[1].Children = new List<ParseUnit>() { Expressions[0] };
            // level 2: Function call, Subscript, Member access
            Expressions[2].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 2",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[2],
                        Operators[2],
                    }
                },
                Expressions[1]
            };
            Operators[2].Children = new List<ParseUnit>() { FunctionCallSuffix, ArraySubscripting, MemberAccess };
            // level-3 (RTL): Unary plus and minus, Logical NOT and bitwise NOT
            Expressions[3].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 3",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Operators[3],
                        Expressions[3],
                    }
                },
                Expressions[2]
            };
            Operators[3].Children = new List<ParseUnit>() { TokenUnits[TokenType.Plus], TokenUnits[TokenType.Minus], TokenUnits[TokenType.BooleanNot], TokenUnits[TokenType.BitwiseNot] };
            // level-4: not used
            Expressions[4].Children = new List<ParseUnit>() { Expressions[3] };
            // level-5: Multiplication, division, remainder
            Expressions[5].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 5",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[5],
                        Operators[5],
                        Expressions[4]
                    }
                },
                Expressions[4]
            };
            Operators[5].Children = new List<ParseUnit>() { TokenUnits[TokenType.Times], TokenUnits[TokenType.Divide], TokenUnits[TokenType.Mod] };
            // level-6: plus, minus
            Expressions[6].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 6",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[6],
                        Operators[6],
                        Expressions[5]
                    }
                },
                Expressions[5]
            };
            Operators[6].Children = new List<ParseUnit>() { TokenUnits[TokenType.Plus], TokenUnits[TokenType.Minus] };
            // level-7:  Bitwise left shift and right shift 
            Expressions[7].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 7",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[7],
                        Operators[7],
                        Expressions[6]
                    }
                },
                Expressions[6]
            };
            Operators[7].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseLeftShiftUnsigned], TokenUnits[TokenType.BitwiseRightShiftUnsigned], TokenUnits[TokenType.BitwiseLeftShiftSigned], TokenUnits[TokenType.BitwiseRightShiftSigned] };
            // level-8: not used
            Expressions[8].Children = new List<ParseUnit>() { Expressions[7] };
            // level-9: <   <= 	>   >=
            Expressions[9].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 9",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[9],
                        Operators[9],
                        Expressions[8]
                    }
                },
                Expressions[8]
            };
            Operators[9].Children = new List<ParseUnit>() { TokenUnits[TokenType.LessThan], TokenUnits[TokenType.LessEqualThan], TokenUnits[TokenType.GreaterThan], TokenUnits[TokenType.GreaterEqualThan] };
            // level-10:  ==   != 
            Expressions[10].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 10",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[10],
                        Operators[10],
                        Expressions[9]
                    }
                },
                Expressions[9]
            };
            Operators[10].Children = new List<ParseUnit>() { TokenUnits[TokenType.EqualTo], TokenUnits[TokenType.NotEqualTo] };
            // level-11: Bitwise AND
            Expressions[11].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 11",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[11],
                        Operators[11],
                        Expressions[10]
                    }
                },
                Expressions[10]
            };
            Operators[11].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseAnd] };
            // level-12: Bitwise XOR
            Expressions[12].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 12",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[12],
                        Operators[12],
                        Expressions[11]
                    }
                },
                Expressions[11]
            };
            Operators[12].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseXor] };
            // level-13: Bitwise OR 
            Expressions[13].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 13",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[13],
                        Operators[13],
                        Expressions[12]
                    }
                },
                Expressions[12]
            };
            Operators[13].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseOr] };
            // level-14: Logical AND
            Expressions[14].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 14",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[14],
                        Operators[14],
                        Expressions[13]
                    }
                },
                Expressions[13]
            };
            Operators[14].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanAnd] };
            // level-15 (not existed on C/C++): Logical XOR
            Expressions[15].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 15",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[15],
                        Operators[15],
                        Expressions[14]
                    }
                },
                Expressions[14]
            };
            Operators[15].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanXor] };
            // level-16 (corresponding 15): Logical OR
            Expressions[16].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 16",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[16],
                        Operators[16],
                        Expressions[15]
                    }
                },
                Expressions[15]
            };
            Operators[16].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanOr] };
            // level-17 (corresponding 16): assign =
            Expressions[17].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 17",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[17],
                        Operators[17],
                        Expressions[16]
                    }
                },
                Expressions[16]
            };
            Operators[17].Children = new List<ParseUnit>() { TokenUnits[TokenType.Assign] };

            // level-18 (corresponding 17): not used
            if (OPERATOR_PRECEDENCE_LEVEL != 18)
            {
                throw  new Exception("Assert failed!");
            } 
             
             

            return MainProgram;

        }

    }
}
