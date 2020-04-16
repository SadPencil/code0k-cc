using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace code0k_cc
{
    class Parser
    {
        private static ParseUnit RootParseUnit { get; } = GetRootParseUnit();

        static ParseInstance Parse(IEnumerable<Token> tokens)
        {
            IReadOnlyList<Token> tokenList = tokens.ToList();
            var ret = _Parse(RootParseUnit, tokenList, 0);
            if (ret.Success)
            {
                return ret.ResultInstance;
            }
            else
            {
                throw new Exception("Failed at Parsing <InstanceName>, near <TokenName> at around <LocationInSourceCode>");
                //todo
            }

        }

        private static ParseResult _Parse(ParseUnit unit, IReadOnlyList<Token> tokenList, int pos)
        {
            var ret = new ParseResult() { Position = pos, ResultInstance = new ParseInstance() { ParseUnit = unit }, Success = false };

            int matchCount = 0;

            // try match


        }

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
                ChildType = ParseUnitChildType.OneChild,
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
            ParseUnit MainProgramItem = new ParseUnit();

            ParseUnit FunctionDeclaration = new ParseUnit();
            ParseUnit FunctionImplementation = new ParseUnit();

            ParseUnit TypeUnit = new ParseUnit();

            ParseUnit FunctionDeclarationArguments = new ParseUnit();
            ParseUnit FunctionArgumentLoop = new ParseUnit();
            ParseUnit FunctionArgumentUnit = new ParseUnit(); 

            ParseUnit StatementBody = new ParseUnit();
            ParseUnit Statement = new ParseUnit();

            ParseUnit DescriptionTokens = new ParseUnit();
            ParseUnit DescriptionTokenUnit = new ParseUnit();

            ParseUnit DefinitionStatement = new ParseUnit();
            ParseUnit IfStatement = new ParseUnit();
            ParseUnit OptionalElseStatement = new ParseUnit();
            ParseUnit ForStatement = new ParseUnit();
            ParseUnit WhileStatement = new ParseUnit();
            ParseUnit CompoundStatement = new ParseUnit();

            ParseUnit FunctionCall = new ParseUnit();
            ParseUnit FunctionCallArgument = new ParseUnit();
            ParseUnit FunctionCallArgumentLoop = new ParseUnit();
            ParseUnit FunctionCallArgumentItem = new ParseUnit();
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

            // write the parse unit
            MainProgram.Name = "Main Program";
            MainProgram.Type = ParseUnitType.Single;
            MainProgram.ChildType = ParseUnitChildType.AllChild;
            MainProgram.Children = new List<ParseUnit>()
            {
                MainProgramItem,
                MainProgram,
            };

            MainProgramItem.Name = "Main Program Item";
            MainProgramItem.Type = ParseUnitType.Single;
            MainProgramItem.ChildType = ParseUnitChildType.OneChild;
            MainProgramItem.Children = new List<ParseUnit>()
            {
                FunctionImplementation,
                FunctionDeclaration,
                DefinitionStatement,
            };  

            FunctionDeclaration.Name = "Function Declaration";
            FunctionDeclaration.Type = ParseUnitType.Single;
            FunctionDeclaration.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclaration.Children = new List<ParseUnit>()
            {
                TypeUnit,
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionDeclarationArguments,
                TokenUnits[TokenType.RightBracket]
            };

            TypeUnit.Name = "Type Name";
            TypeUnit.Type = ParseUnitType.Single;
            TypeUnit.ChildType = ParseUnitChildType.AllChild;
            TypeUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier]
            };

            FunctionImplementation.Name = "Function Implementation";
            FunctionImplementation.Type = ParseUnitType.Single;
            FunctionImplementation.ChildType = ParseUnitChildType.AllChild;
            FunctionImplementation.Children = new List<ParseUnit>()
            {
                TypeUnit,
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
                FunctionArgumentLoop,
            };

            FunctionArgumentUnit.Name = "Function Argument Unit";
            FunctionArgumentUnit.Type = ParseUnitType.Single;
            FunctionArgumentUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentUnit.Children = new List<ParseUnit>()
            {
                DescriptionTokens,
                TypeUnit,
                TokenUnits[TokenType.Identifier]
            };

            FunctionArgumentLoop.Name = "Function Argument Loop";
            FunctionArgumentLoop.Type = ParseUnitType.SingleOptional;
            FunctionArgumentLoop.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                FunctionDeclarationArguments
            };


            StatementBody.Name = "Statement Body";
            StatementBody.Type = ParseUnitType.SingleOptional;
            StatementBody.ChildType = ParseUnitChildType.AllChild;
            StatementBody.Children = new List<ParseUnit>()
            {
                Statement,
                TokenUnits[TokenType.Semicolon],
                StatementBody
            };

            Statement.Name = "Statement";
            Statement.Type = ParseUnitType.SingleOptional;
            Statement.ChildType = ParseUnitChildType.OneChild;
            Statement.Children = new List<ParseUnit>()
            {
                DefinitionStatement,
                IfStatement,
                ForStatement,
                WhileStatement,
                Expression,
                CompoundStatement,
            };

            DefinitionStatement.Name = "Definition Statement";
            DefinitionStatement.Type = ParseUnitType.Single;
            DefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            DefinitionStatement.Children = new List<ParseUnit>()
            {
                DescriptionTokens,
                TypeUnit,
                Expression,
                TokenUnits[TokenType.Assign],
                Expression
            };

            DescriptionTokens.Name = "Definition Description";
            DescriptionTokens.Type = ParseUnitType.SingleOptional;
            DescriptionTokens.ChildType = ParseUnitChildType.AllChild;
            DescriptionTokens.Children = new List<ParseUnit>()
            {
                DescriptionTokenUnit,
                DescriptionTokens,
            };

            DescriptionTokenUnit.Name = "Definition Description Unit";
            DescriptionTokenUnit.Type = ParseUnitType.Single;
            DescriptionTokenUnit.ChildType = ParseUnitChildType.OneChild;
            DescriptionTokenUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Input],
                TokenUnits[TokenType.NizkInput],
                TokenUnits[TokenType.Output],
                TokenUnits[TokenType.Const],
                TokenUnits[TokenType.Ref],
            };

            IfStatement.Name = "If Statement";
            IfStatement.Type = ParseUnitType.Single;
            IfStatement.ChildType = ParseUnitChildType.AllChild;
            IfStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.If],
                Expression,
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
                Expression,
                TokenUnits[TokenType.Assign],
                Expression,
                TokenUnits[TokenType.To],
                Expression,
                TokenUnits[TokenType.Do],
                CompoundStatement
            };

            WhileStatement.Name = "While Statement";
            WhileStatement.Type = ParseUnitType.Single;
            WhileStatement.ChildType = ParseUnitChildType.AllChild;
            WhileStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.While],
                Expression,
                TokenUnits[TokenType.Max],
                Expression,
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

            //LeftValue.Name = "Left Value";
            //LeftValue.Type = ParseUnitType.Single;
            //LeftValue.ChildType = ParseUnitChildType.AllChild;
            //LeftValue.Children = new List<ParseUnit>()
            //{
            //    TokenUnits[TokenType.Identifier],
            //    new ParseUnit()
            //    {
            //        Name="Left Value",
            //        Type = ParseUnitType.MultipleOptional,
            //        ChildType = ParseUnitChildType.OneChild,
            //        Children =  new List<ParseUnit>()
            //        {
            //            MemberAccess,
            //            ArraySubscripting
            //        }
            //    }
            //};

            // the expression part is written according to the following precedence
            // https://en.cppreference.com/w/c/language/operator_precedence
            // Accessed at 2020-04-16

            // so I will follow the number from 1 to 17
            // although some of the operators are not implemented by now
            // these number are still reserved
            // Expression[0] means the identifier or number

            Expression.Name = "Expression";
            Expression.Type = ParseUnitType.Single;
            Expression.ChildType = ParseUnitChildType.OneChild;
            Expression.Children = new List<ParseUnit>()
            {
                Expressions[OPERATOR_PRECEDENCE_LEVEL - 1]
            };

            foreach (var i in Enumerable.Range(0, OPERATOR_PRECEDENCE_LEVEL))
            {
                Expressions[i].Name = "Expression Level " + i.ToString(CultureInfo.InvariantCulture);
                Expressions[i].Type = ParseUnitType.Single;
                Expressions[i].ChildType = ParseUnitChildType.OneChild;
                Operators[i].Name = "Operator Level " + i.ToString(CultureInfo.InvariantCulture);
                Operators[i].Type = ParseUnitType.Single;
                Operators[i].ChildType = ParseUnitChildType.OneChild;
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
            Operators[2].Children = new List<ParseUnit>() { FunctionCall, ArraySubscripting, MemberAccess };

            FunctionCall.Name = "Function Call";
            FunctionCall.Type = ParseUnitType.Single;
            FunctionCall.ChildType = ParseUnitChildType.AllChild;
            FunctionCall.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                FunctionCallArgument,
                TokenUnits[TokenType.RightBracket],
                //new ParseUnit()
                //{
                //    Name = "Function Call Argument",
                //    Type = ParseUnitType.SingleOptional,
                //    ChildType = ParseUnitChildType.AllChild,
                //    Children = new List<ParseUnit>()
                //    {
                //        new ParseUnit()
                //        {
                //            Name = "Function Call Argument Unit",
                //            Type = ParseUnitType.MultipleOptional,
                //            ChildType = ParseUnitChildType.AllChild,
                //            Children = new List<ParseUnit>()
                //            {
                //                Expression,
                //                TokenUnits[TokenType.Comma]
                //            }
                //        },
                //        new ParseUnit()
                //        {
                //            Name = "Function Call Argument Unit",
                //            Type = ParseUnitType.Single,
                //            ChildType = ParseUnitChildType.AllChild,
                //            Children =  new List<ParseUnit>()
                //            {
                //                Expression
                //            }
                //        }
                //    }
                //},
            };
            FunctionCallArgument.Name = "Function Call Argument";
            FunctionCallArgument.Type = ParseUnitType.SingleOptional;
            FunctionCallArgument.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgument.Children = new List<ParseUnit>()
            {
                FunctionCallArgumentItem,
                FunctionCallArgumentLoop
            };

            FunctionCallArgumentLoop.Name = "Function Call Argument Loop";
            FunctionCallArgumentLoop.Type = ParseUnitType.SingleOptional;
            FunctionCallArgumentLoop.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgumentLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                FunctionCallArgument, 
            };

            FunctionCallArgumentItem.Name = "Function Call Argument Item";
            FunctionCallArgumentItem.Type = ParseUnitType.Single;
            FunctionCallArgumentItem.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgumentItem.Children = new List<ParseUnit>()
            {
                Expression
            };

            ArraySubscripting.Name = "Array Subscripting";
            ArraySubscripting.Type = ParseUnitType.Single;
            ArraySubscripting.ChildType = ParseUnitChildType.AllChild;
            ArraySubscripting.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftSquareBracket],
                Expression,
                TokenUnits[TokenType.RightSquareBracket]
            };

            MemberAccess.Name = "Member Access";
            MemberAccess.Type = ParseUnitType.Single;
            MemberAccess.ChildType = ParseUnitChildType.AllChild;
            MemberAccess.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Dot],
                Expressions[2]
            };


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
            // level-17 (RTL) (corresponding 16): assign =
            Expressions[17].Children = new List<ParseUnit>()
            {
                new ParseUnit()
                {
                    Name = "Expression Level 17",
                    Type = ParseUnitType.Single,
                    ChildType = ParseUnitChildType.AllChild,
                    Children = new List<ParseUnit>()
                    {
                        Expressions[16],
                        Operators[17],
                        Expressions[17]
                    }
                },
                Expressions[16]
            };
            Operators[17].Children = new List<ParseUnit>() { TokenUnits[TokenType.Assign] };

            // level-18 (corresponding 17): not used
            if (OPERATOR_PRECEDENCE_LEVEL != 18)
            {
                throw new Exception("Assert failed!");
            }



            return MainProgram;

        }

    }
}
