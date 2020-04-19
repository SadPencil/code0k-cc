using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using code0k_cc.Lex;
using code0k_cc.Runtime;
using code0k_cc.Runtime.ExeArg;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Type;

namespace code0k_cc.Parse
{
    class Parser
    {
        private static ParseUnit RootParseUnit { get; } = GetRootParseUnit();

        internal static ParseInstance Parse(in IEnumerable<Token> tokens)
        {
            IReadOnlyList<Token> tokenList = tokens.ToList();
            var ret = Parse(RootParseUnit, tokenList, 0, 0);
            if (ret.Success)
            {
                return ret.ResultInstance;
            }
            else
            {
                //todo: to make this error message more reliable, compute FIRST and FOLLOW
                throw new Exception(
                    $"Failed at Parsing {ret.ResultInstance?.ParseUnit?.Name}, at row {ret.Row} col {ret.Column}");
            }

        }

        private static ParseResult Parse(in ParseUnit unit, in IReadOnlyList<Token> tokenList, in int pos, in int depth)
        {

            {
                //debug
                if (depth > 1000)
                {
                    throw new Exception("stop");
                }
                Console.Write("\t" + depth);
                for (int kkk = 0; kkk < depth; ++kkk)
                {
                    Console.Write(" ");
                }

                Console.Write(unit.Name);

                Console.Write("\t");
                Console.Write(tokenList.ElementAtOrDefault(pos)?.Value);
                Console.Write(" [");
                Console.Write(pos);
                Console.Write("]");
                Console.WriteLine();
            }

            Debug.Assert(tokenList.Last().TokenType == TokenType.EOL);
            var token = tokenList[pos];

            if (unit.ChildType == ParseUnitChildType.Terminal)
            {
                // match the token
                if (token.TokenType == unit.TerminalTokenType)
                {
                    // matched
                    var ret = new ParseResult()
                    {
                        Position = pos + 1,
                        Success = true,
                        ResultInstance = new ParseInstance()
                        {
                            Children = null,
                            ParseUnit = unit,
                            Token = token,
                        },
                        Row = token.Row,
                        Column = token.Column,
                    };
                    return ret;

                }
                else
                // failed
                {
                    if (unit.Type == ParseUnitType.SingleOptional)
                    {
                        // match null
                        var ret = new ParseResult()
                        {
                            Position = pos,
                            Success = true,
                            ResultInstance = null,
                            Row = token.Row,
                            Column = token.Column,
                        };
                        return ret;
                    }
                    else if (unit.Type == ParseUnitType.Single)
                    {
                        // failed
                        var ret = new ParseResult()
                        {
                            Position = pos,
                            Success = false,
                            ResultInstance = new ParseInstance()
                            {
                                Children = null,
                                ParseUnit = unit,
                                Token = null
                            },
                            Row = token.Row,
                            Column = token.Column,
                        };
                        return ret;
                    }
                    else
                    {
                        throw new Exception("Assert failed!");
                    }
                }
            }
            else if (unit.ChildType == ParseUnitChildType.AllChild)
            {
                ParseResult badResult = null;
                int newPos = pos;
                var newRowCol = (Row: token.Row, Column: token.Column);
                List<ParseInstance> children = new List<ParseInstance>();
                foreach (var unitChild in unit.Children)
                {
                    var ret = Parse(unitChild, tokenList, newPos, depth + 1);
                    if (!ret.Success)
                    {
                        badResult = ret;
                        break;
                    }
                    else
                    {
                        newPos = ret.Position;
                        newRowCol = (ret.Row, ret.Column);
                        children.Add(ret.ResultInstance);
                    }
                }

                if (badResult != null)
                {
                    // on failure
                    if (unit.Type == ParseUnitType.SingleOptional)
                    {
                        // match null
                        var ret = new ParseResult()
                        {
                            Position = pos,
                            Success = true,
                            ResultInstance = null,
                            Row = token.Row,
                            Column = token.Column,
                        };
                        return ret;
                    }
                    else if (unit.Type == ParseUnitType.Single)
                    {
                        // failed
                        return badResult;
                    }
                    else
                    {
                        throw new Exception("Assert failed!");
                    }
                }
                else
                {
                    // on success
                    var ret = new ParseResult()
                    {
                        Position = newPos,
                        Success = true,
                        ResultInstance = new ParseInstance()
                        {
                            Children = children,
                            ParseUnit = unit,
                            Token = null
                        },
                        Row = newRowCol.Row,
                        Column = newRowCol.Column,
                    };
                    return ret;
                }

            }
            else if (unit.ChildType == ParseUnitChildType.OneChild)
            {
                ParseResult goodResult = null;
                ParseResult lastResult = null;
                foreach (var unitChild in unit.Children)
                {
                    lastResult = Parse(unitChild, tokenList, pos, depth + 1);
                    if (lastResult.Success)
                    {
                        goodResult = lastResult;
                        break;
                    }
                }

                if (goodResult == null)
                {
                    // not matched
                    if (unit.Type == ParseUnitType.SingleOptional)
                    {
                        // match null
                        var ret = new ParseResult()
                        {
                            Position = pos,
                            Success = true,
                            ResultInstance = null,
                            Row = token.Row,
                            Column = token.Column,
                        };
                        return ret;
                    }
                    else if (unit.Type == ParseUnitType.Single)
                    {
                        // failed
                        return lastResult;
                    }
                    else
                    {
                        throw new Exception("Assert failed!");
                    }
                }
                else
                {
                    // matched
                    var ret = new ParseResult()
                    {
                        Position = goodResult.Position,
                        Success = true,
                        ResultInstance = new ParseInstance()
                        {
                            Children = new List<ParseInstance>() { goodResult.ResultInstance },
                            ParseUnit = unit,
                            Token = null
                        },
                        Row = goodResult.Row,
                        Column = goodResult.Column,
                    };
                    return ret;
                }


            }
            else
            {
                throw new Exception("Assert failed!");
            }
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
                    ChildType = ParseUnitChildType.Terminal,
                    TerminalTokenType = tokenType
                });
            }

            ParseUnit eolUnit = new ParseUnit()
            {
                Name = "Token " + TokenType.EOL.Name,
                Type = ParseUnitType.Single,
                ChildType = ParseUnitChildType.Terminal,
                TerminalTokenType = TokenType.EOL
            };

            ParseUnit MainProgram = new ParseUnit();
            ParseUnit MainProgramItem = new ParseUnit();
            ParseUnit MainProgramLoop = new ParseUnit();

            ParseUnit GlobalDefinitionStatement = new ParseUnit();

            ParseUnit FunctionDeclaration = new ParseUnit();
            ParseUnit FunctionImplementation = new ParseUnit();

            ParseUnit TypeUnit = new ParseUnit();
            ParseUnit TypeUnitGenerics = new ParseUnit();
            ParseUnit TypeUnitGenericsLoop = new ParseUnit();

            ParseUnit LeftValue = new ParseUnit();
            ParseUnit LeftValueSuffixLoop = new ParseUnit();
            ParseUnit LeftValueSuffixItem = new ParseUnit();

            ParseUnit FunctionDeclarationArguments = new ParseUnit();
            ParseUnit FunctionDeclarationArgumentLoop = new ParseUnit();
            ParseUnit FunctionDeclarationArgumentUnit = new ParseUnit();

            ParseUnit StatementBody = new ParseUnit();
            ParseUnit Statement = new ParseUnit();
            ParseUnit StatementSemicolonCollection = new ParseUnit();
            ParseUnit StatementSemicolon = new ParseUnit();

            ParseUnit DefinitionStatement = new ParseUnit();
            ParseUnit IfStatement = new ParseUnit();
            ParseUnit OptionalElseStatement = new ParseUnit();
            ParseUnit ForStatement = new ParseUnit();
            ParseUnit WhileStatement = new ParseUnit();
            ParseUnit CompoundStatement = new ParseUnit();
            ParseUnit ReturnStatement = new ParseUnit();
            ParseUnit BreakStatement = new ParseUnit();
            ParseUnit ContinueStatement = new ParseUnit();

            ParseUnit FunctionCall = new ParseUnit();
            ParseUnit FunctionCallArgument = new ParseUnit();
            ParseUnit FunctionCallArgumentLoop = new ParseUnit();
            ParseUnit FunctionCallArgumentItem = new ParseUnit();
            ParseUnit ArraySubscripting = new ParseUnit();
            ParseUnit MemberAccess = new ParseUnit();

            ParseUnit Expression = new ParseUnit();
            ParseUnit BracketExpression = new ParseUnit();

            const int OPERATOR_PRECEDENCE_LEVEL = 18;
            ParseUnit[] Expressions = new ParseUnit[OPERATOR_PRECEDENCE_LEVEL];
            ParseUnit[] Operators = new ParseUnit[OPERATOR_PRECEDENCE_LEVEL];
            ParseUnit[] ExpressionsHelper = new ParseUnit[OPERATOR_PRECEDENCE_LEVEL];

            foreach (var i in Enumerable.Range(0, OPERATOR_PRECEDENCE_LEVEL))
            {
                Expressions[i] = new ParseUnit();
                Operators[i] = new ParseUnit();
                ExpressionsHelper[i] = new ParseUnit();
            }

            // write the parse unit & execution function
            MainProgram.Name = "Main Program";
            MainProgram.Type = ParseUnitType.Single;
            MainProgram.ChildType = ParseUnitChildType.AllChild;
            MainProgram.Children = new List<ParseUnit>()
            {
                MainProgramItem,
                MainProgramLoop,
                eolUnit,
            };
            MainProgram.Execute = arg =>
           {
               //block: provide built-in types

               //todo: redundancy code here.

               // prepare environment
               var mainBlock = new EnvironmentBlock() { ParseInstance = arg.Instance, ParentBlock = arg.Block, ReturnBlock = arg.Block };

               foreach (var instanceChild in arg.Instance.Children)
               {
                   _ = instanceChild?.Execute(new ExeArg() { Block = mainBlock });
               }

               // find & execute "main"
               var mainFunc = (FunctionDeclarationValue) mainBlock.GetVariableRef("main", false).Variable.Value;

               if (mainFunc.Instance == null)
               {
                   throw new Exception($"Unimplemented function \"{mainFunc.FunctionName}\"");
               }

               // create new block
               EnvironmentBlock newFuncBlock = new EnvironmentBlock()
               {
                   ParentBlock = mainFunc.ParentBlock,
                   ParseInstance = mainFunc.Instance,
                   ReturnBlock = mainBlock,
               };

               // execute function
               return mainFunc.Instance.Execute(new ExeArg() { Block = newFuncBlock });
           };

            MainProgramItem.Name = "Main Program Item";
            MainProgramItem.Type = ParseUnitType.Single;
            MainProgramItem.ChildType = ParseUnitChildType.OneChild;
            MainProgramItem.Children = new List<ParseUnit>()
            {
                GlobalDefinitionStatement,
                FunctionDeclaration,
                FunctionImplementation,
            };
            MainProgramItem.Execute = arg => arg.Instance.Children[0].Execute(arg);

            MainProgramLoop.Name = "Main Program Loop";
            MainProgramLoop.Type = ParseUnitType.SingleOptional;
            MainProgramLoop.ChildType = ParseUnitChildType.AllChild;
            MainProgramLoop.Children = new List<ParseUnit>()
            {
                MainProgramItem,
                MainProgramLoop,
            };
            MainProgramLoop.Execute = arg => arg.Instance.Children[0].Execute(arg);


            FunctionDeclaration.Name = "Function Declaration";
            FunctionDeclaration.Type = ParseUnitType.Single;
            FunctionDeclaration.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclaration.Children = new List<ParseUnit>()
            {
                TypeUnit,
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionDeclarationArguments,
                TokenUnits[TokenType.RightBracket],
                TokenUnits[TokenType.Semicolon]
            };
            FunctionDeclaration.Execute = (arg) =>
            {
                var typeUnitResult = arg.Instance.Children[0].Execute(arg).TypeUnitResult;
                var funRetNType = NType.GetNType(typeUnitResult);

                var funName = arg.Instance.Children[1].Token.Value;

                var funDeclareArgsRaw = instance.Children[3]?.Execute(block, null);

                var argList = new TFunctionDeclarationArguments();

                if (funDeclareArgsRaw != null)
                {
                    var funDeclareArgsT = (TFunctionDeclarationArguments) funDeclareArgsRaw;
                    argList.Arguments = funDeclareArgsT.Arguments;
                }

                var funT = new FunctionDeclarationValue() { Arguments = argList, FunctionName = funName, ReturnType = funRetNType, Instance = null };

                arg.Block.AddVariable(funName, new Variable() { Type = NType.Function, Value = funT });

                return null;
            };

            TypeUnit.Name = "Type Unit";
            TypeUnit.Type = ParseUnitType.Single;
            TypeUnit.ChildType = ParseUnitChildType.AllChild;
            TypeUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TypeUnitGenerics,
            };
            TypeUnit.Execute = arg => new ExeResult()
            {
                TypeUnitResult = new TypeResult()
                {
                    TypeName = arg.Instance.Children[0].Token.Value,
                    Generics = arg.Instance.Children[1]?.Execute(new ExeArg() { Block = arg.Block })?.GenericsTypeResult
                }
            };

            TypeUnitGenerics.Name = "Type Unit Generics";
            TypeUnitGenerics.Type = ParseUnitType.SingleOptional;
            TypeUnitGenerics.ChildType = ParseUnitChildType.AllChild;
            TypeUnitGenerics.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LessThan],
                TypeUnit,
                TypeUnitGenericsLoop,
                TokenUnits[TokenType.GreaterThan],
            };
            TypeUnitGenerics.Execute = arg =>
            {
                TypeResult type1 = arg.Instance.Children[1].Execute(new ExeArg() { Block = arg.Block }).TypeUnitResult;
                GenericsTypeResult typeRest = arg.Instance.Children[2]?.Execute(new ExeArg() { Block = arg.Block })?.GenericsTypeResult;

                GenericsTypeResult retResult = new GenericsTypeResult() { Types = new List<TypeResult>() { type1 } };

                if (typeRest != null)
                {
                    if (typeRest.Types.Count > 0)
                    {
                        retResult.Types.AddRange(typeRest.Types);
                    }
                }

                return new ExeResult() { GenericsTypeResult = retResult };
            };

            TypeUnitGenericsLoop.Name = "Type Unit Generics Loop";
            TypeUnitGenericsLoop.Type = ParseUnitType.SingleOptional;
            TypeUnitGenericsLoop.ChildType = ParseUnitChildType.AllChild;
            TypeUnitGenericsLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                TypeUnit,
                TypeUnitGenericsLoop
            };
            TypeUnitGenericsLoop.Execute = TypeUnitGenerics.Execute; //happened to be same


            FunctionImplementation.Name = "Function Implementation";
            FunctionImplementation.Type = ParseUnitType.Single;
            FunctionImplementation.ChildType = ParseUnitChildType.AllChild;
            FunctionImplementation.Children = new List<ParseUnit>()
            {
                FunctionDeclaration,
                CompoundStatement
            };
            FunctionImplementation.Execute = arg =>
            {
                // 1. re-declare the function if it is already declared
                // 2. set the FunctionDeclarationValue.Instance to the compound statement

                //todo
            };


            FunctionDeclarationArguments.Name = "Function Declaration Arguments";
            FunctionDeclarationArguments.Type = ParseUnitType.SingleOptional;
            FunctionDeclarationArguments.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArguments.Children = new List<ParseUnit>()
            {
                FunctionDeclarationArgumentUnit,
                FunctionDeclarationArgumentLoop,
            };
            FunctionDeclarationArguments.Execute = (instance, block, arg) =>
            {
                //link the list
                var thisDecArgT = (TFunctionDeclarationArguments) instance.Children[0].Execute(block, arg);

                var thatDecArgTRaw = instance.Children[1]?.Execute(block, arg);
                if (thatDecArgTRaw != null)
                {
                    var thatDecArgT = (TFunctionDeclarationArguments) thatDecArgTRaw;
                    // no clone at this time
                    thisDecArgT.Arguments = thisDecArgT.Arguments.Concat(thatDecArgT.Arguments).ToList();
                }

                return thisDecArgT;
            };

            FunctionDeclarationArgumentUnit.Name = "Function Declaration Argument Unit";
            FunctionDeclarationArgumentUnit.Type = ParseUnitType.Single;
            FunctionDeclarationArgumentUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArgumentUnit.Children = new List<ParseUnit>()
            {
                TypeUnit,
                TokenUnits[TokenType.Identifier]
            };
            FunctionDeclarationArgumentUnit.Execute = (instance, block, arg) =>
            {
                var funDecArgT = (TTypeOfType) instance.Children[0].Execute(block, null);
                var funArgNameT = (TString) instance.Children[1].Execute(block, null);
                var retT = new TFunctionDeclarationArguments() { Arguments = new List<(TTypeOfType Type, string VarName)>() { (funDecArgT, funArgNameT.Value) } };
                return retT;
            };

            FunctionDeclarationArgumentLoop.Name = "Function Declaration Argument Loop";
            FunctionDeclarationArgumentLoop.Type = ParseUnitType.SingleOptional;
            FunctionDeclarationArgumentLoop.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArgumentLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                FunctionDeclarationArguments
            };
            FunctionDeclarationArgumentLoop.Execute = (instance, block, arg) => instance.Children[1]?.Execute(block, arg);


            StatementBody.Name = "Statement Body";
            StatementBody.Type = ParseUnitType.SingleOptional;
            StatementBody.ChildType = ParseUnitChildType.AllChild;
            StatementBody.Children = new List<ParseUnit>()
            {
                Statement,
                StatementBody
            };
            StatementBody.Execute = (instance, block, arg) => new TVoid();

            StatementSemicolon.Name = "Statement Semicolon";
            StatementSemicolon.Type = ParseUnitType.Single;
            StatementSemicolon.ChildType = ParseUnitChildType.AllChild;
            StatementSemicolon.Children = new List<ParseUnit>()
            {
                StatementSemicolonCollection,
                TokenUnits[TokenType.Semicolon],
            };
            StatementSemicolon.Execute = (instance, block, arg) => instance.Children[0].Execute(block, arg);

            StatementSemicolonCollection.Name = "Statement Semicolon Collection";
            StatementSemicolonCollection.Type = ParseUnitType.SingleOptional; // null statement included
            StatementSemicolonCollection.ChildType = ParseUnitChildType.OneChild;
            StatementSemicolonCollection.Children = new List<ParseUnit>()
            {
                BreakStatement,
                ContinueStatement,
                ReturnStatement,
                DefinitionStatement,
                Expression
            };
            StatementSemicolonCollection.Execute = (instance, block, arg) =>
            {
                if (instance.Children.Count > 0)
                {
                    return instance.Children[0].Execute(block, arg);
                }
                else
                {
                    return new TVoid();
                }
            };


            Statement.Name = "Statement";
            Statement.Type = ParseUnitType.Single;
            Statement.ChildType = ParseUnitChildType.OneChild;
            Statement.Children = new List<ParseUnit>()
            {
                CompoundStatement,
                IfStatement,
                ForStatement,
                WhileStatement,
                StatementSemicolon,
            };
            Statement.Execute = (instance, block, arg) => instance.Children[0].Execute(block, arg);

            GlobalDefinitionStatement.Name = "Global Definition Statement";
            GlobalDefinitionStatement.Type = ParseUnitType.Single;
            GlobalDefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            GlobalDefinitionStatement.Children = new List<ParseUnit>()
            {
                DefinitionStatement,
                TokenUnits[TokenType.Semicolon],
            };
            GlobalDefinitionStatement.Execute = (instance, block, arg) => instance.Children[0].Execute(block, arg);

            ReturnStatement.Name = "Return Statement";
            ReturnStatement.Type = ParseUnitType.Single;
            ReturnStatement.ChildType = ParseUnitChildType.AllChild;
            ReturnStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Return],
                Expression,
            };
            //todo execute


            BreakStatement.Name = "Break Statement";
            BreakStatement.Type = ParseUnitType.Single;
            BreakStatement.ChildType = ParseUnitChildType.AllChild;
            BreakStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Break],
            };
            //todo execute

            ContinueStatement.Name = "Continue Statement";
            ContinueStatement.Type = ParseUnitType.Single;
            ContinueStatement.ChildType = ParseUnitChildType.AllChild;
            ContinueStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Continue],
            };
            //todo execute

            DefinitionStatement.Name = "Definition Statement";
            DefinitionStatement.Type = ParseUnitType.Single;
            DefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            DefinitionStatement.Children = new List<ParseUnit>()
            {
                TypeUnit,
                LeftValue,
                TokenUnits[TokenType.Assign],
                Expression
            };
            DefinitionStatement.Execute = (instance, block, arg) =>
            {
                var varNameT = (TString) ( instance.Children[1].Execute(block, null) );
                var varName = varNameT.Value;

                var typeT = (TTypeOfType) ( instance.Children[0].Execute(block, null) );

                var expressionValueT = instance.Children[3].Execute(block, null);

                if (!typeT.IsImplicitConvertible(expressionValueT))
                {
                    throw new Exception($"Unexpected type when assigning variable \"{varName}\"." + Environment.NewLine +
                                        $"Supposed to be \"{ typeT.GetTypeCodeName() }\", got \"{typeT.TypeCodeName}\" here.");
                }

                //todo do Implicit Convert

                block.Variables.Add(varName, expressionValueT);

                return null;

            };


            IfStatement.Name = "If Statement";
            IfStatement.Type = ParseUnitType.Single;
            IfStatement.ChildType = ParseUnitChildType.AllChild;
            IfStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.If],
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket],
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
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.Semicolon],
                Expression,
                TokenUnits[TokenType.Semicolon],
                Expression,
                TokenUnits[TokenType.RightBracket],
                TokenUnits[TokenType.Max],
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket],
                CompoundStatement
            };


            WhileStatement.Name = "While Statement";
            WhileStatement.Type = ParseUnitType.Single;
            WhileStatement.ChildType = ParseUnitChildType.AllChild;
            WhileStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.While],
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket],
                TokenUnits[TokenType.Max],
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket],
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
            //todo exe

            LeftValue.Name = "Left Value";
            LeftValue.Type = ParseUnitType.Single;
            LeftValue.ChildType = ParseUnitChildType.AllChild;
            LeftValue.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                LeftValueSuffixLoop,
            };
            LeftValueSuffixLoop.Name = "Left Value Suffix";
            LeftValueSuffixLoop.Type = ParseUnitType.SingleOptional;
            LeftValueSuffixLoop.ChildType = ParseUnitChildType.AllChild;
            LeftValueSuffixLoop.Children = new List<ParseUnit>()
            {
                LeftValueSuffixItem,
                LeftValueSuffixLoop,
            };
            LeftValueSuffixItem.Name = "Left Value Suffix";
            LeftValueSuffixItem.Type = ParseUnitType.Single;
            LeftValueSuffixItem.ChildType = ParseUnitChildType.OneChild;
            LeftValueSuffixItem.Children = new List<ParseUnit>()
            {
                ArraySubscripting,
                MemberAccess,
            };

            // the expression part is written according to the following precedence
            // https://en.cppreference.com/w/c/language/operator_precedence
            // Accessed at 2020-04-16

            // so I will follow the number from 1 to 17
            // although some of the operators are not implemented by now
            // these number are still reserved
            // Expression[0] means the identifier or number or bracket expression

            Expression.Name = "Expression";
            Expression.Type = ParseUnitType.Single;
            Expression.ChildType = ParseUnitChildType.OneChild;
            Expression.Children = new List<ParseUnit>()
            {
                Expressions[OPERATOR_PRECEDENCE_LEVEL - 1]
            };
            Expression.Execute = arg => new ExeResult() { ExpressionResult = arg.Instance.Children[0].Execute(new ExeArg() { Block = arg.Block }).ExpressionResult };

            BracketExpression.Name = "Bracket Expression";
            BracketExpression.Type = ParseUnitType.Single;
            BracketExpression.ChildType = ParseUnitChildType.AllChild;
            BracketExpression.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket]
            };
            BracketExpression.Execute = arg => new ExeResult() { ExpressionResult = arg.Instance.Children[1].Execute(new ExeArg() { Block = arg.Block }).ExpressionResult };


            foreach (var i in Enumerable.Range(0, OPERATOR_PRECEDENCE_LEVEL))
            {
                // some level's expression are not followed these settings and it will be overriden later in the code
                Expressions[i].Name = "Expression Level " + i.ToString(CultureInfo.InvariantCulture);
                Expressions[i].Type = ParseUnitType.Single;
                Expressions[i].ChildType = ParseUnitChildType.AllChild;

                Operators[i].Name = "Operator Level " + i.ToString(CultureInfo.InvariantCulture);
                Operators[i].Type = ParseUnitType.Single;
                Operators[i].ChildType = ParseUnitChildType.OneChild;

                ExpressionsHelper[i].Name = "Expression Level " + i.ToString(CultureInfo.InvariantCulture);
                ExpressionsHelper[i].Type = ParseUnitType.SingleOptional;
                ExpressionsHelper[i].ChildType = ParseUnitChildType.AllChild;
            }

            // most of them are associated left-to-right
            // except for level 3 and level 16, which are right-to-left

            // level 0: Identifier or number 
            Expressions[0].Children = new List<ParseUnit>() { Operators[0] };
            Expressions[0].Execute = arg => arg.Instance.Children[0].Execute(arg);

            Operators[0].Children = new List<ParseUnit>() { TokenUnits[TokenType.Identifier], TokenUnits[TokenType.Number], BracketExpression };
            Operators[0].Execute = arg =>
            {
                if (arg.Instance.Children[0].ParseUnit == TokenUnits[TokenType.Identifier])
                {
                    return new ExeResult()
                    {
                        ExpressionResult = new ExpressionResult() { VariableRef = arg.Block.GetVariableRef(arg.Instance.Children[0].Execute(arg).TokenResult.Token.Value, false) }
                    };
                }
                else if (arg.Instance.Children[0].ParseUnit == TokenUnits[TokenType.Number])
                {
                    string numberStr = arg.Instance.Children[0].Execute(arg).TokenResult.Token.Value;
                    //todo: currently, only support UInt32, will add other number type later
                    var retVar = NType.UInt32.Parse(numberStr);
                    return new ExeResult()
                    {
                        ExpressionResult = new ExpressionResult() { VariableRef = retVar.GetVariableRef() }
                    };
                }
                else if (arg.Instance.Children[0].ParseUnit == BracketExpression)
                {
                    return arg.Instance.Children[0].Execute(arg);
                }
                else
                {
                    throw new Exception("Assert failed!");
                }
            };

            // level 1: not used
            Expressions[1].Children = new List<ParseUnit>() { Expressions[0] };
            Expressions[1].Execute = arg => arg.Instance.Children[0].Execute(arg);

            // level 2: Function call, Subscript, Member access
            //
            // note: eliminate left recursion
            // Example:
            //
            // A -> Aα | β
            // 
            // will be converted to
            // 
            // A -> βR
            // R -> αR | ε
            // (where ε stands for null)
            //
            // In this situation, A is Expression[2], α is Operator[2] and β is Expression[1]
            // R is ExpressionHelper[2]

            Expressions[2].Children = new List<ParseUnit>() { Expressions[1], ExpressionsHelper[2] };
            Expressions[2].Execute = arg =>
            {
                var exp = arg.Instance.Children[0].Execute(arg).ExpressionResult;
                var ins = arg.Instance.Children[1];
                while (true)
                {
                    var op = ins?.Children[0];
                    ins = ins?.Children[1];
                    if (op == null) break;
                    //todo exp = op(exp)
                    //todo!!

                    if (op.ParseUnit == FunctionCall)
                    {
                        // get func
                        var functionDefinitionRef = exp.VariableRef;
                        var funcStruct = (FunctionDeclarationValue) functionDefinitionRef.Variable.Value;

                        if (funcStruct.Instance == null)
                        {
                            throw new Exception($"Unimplemented function \"{funcStruct.FunctionName}\"");
                        }

                        // create new block
                        EnvironmentBlock newBlock = new EnvironmentBlock()
                        {
                            ParentBlock = funcStruct.ParentBlock,
                            ParseInstance = funcStruct.Instance,
                            ReturnBlock = arg.Block,
                        };

                        // get & load all params 
                        var paramLoopIns = op.Children[1];
                        int argCount = 0;
                        while (true)
                        {
                            var argItemIns = paramLoopIns?.Children[0];
                            paramLoopIns = paramLoopIns?.Children[1];
                            if (argItemIns == null) break;

                            var argExp = argItemIns.Execute(arg).ExpressionResult;
                            // ByVal : convert type when calling
                            if (funcStruct.Arguments.Count - 1 < argCount)
                            {
                                throw new Exception($"Unexpected function arguments of function \"{funcStruct.FunctionName}\".");
                            }

                            (string argName, NType argNType) = funcStruct.Arguments[argCount];
                            var newArgVal = argExp.VariableRef.Variable.Assign(argNType);

                            //add params
                            newBlock.AddVariable(argName, newArgVal);

                            ++argCount;
                        }

                        if (argCount != funcStruct.Arguments.Count)
                        {
                            throw new Exception($"Unexpected function arguments of function \"{funcStruct.FunctionName}\".");
                        }

                        // execute function
                        return funcStruct.Instance.Execute(new ExeArg() { Block = newBlock });
                    }
                    else if (op.ParseUnit == MemberAccess)
                    {
                        //todo

                    }
                    else if (op.ParseUnit == ArraySubscripting)
                    {
                        //todo
                    }
                    else
                    {
                        throw new Exception("Assert failed!");
                    }


                }

                return new ExeResult() { ExpressionResult = exp };
            };

            ExpressionsHelper[2].Children = new List<ParseUnit>() { Operators[2], ExpressionsHelper[2] };
            Operators[2].Children = new List<ParseUnit>() { FunctionCall, ArraySubscripting, MemberAccess };

            FunctionCall.Name = "Function Call";
            FunctionCall.Type = ParseUnitType.Single;
            FunctionCall.ChildType = ParseUnitChildType.AllChild;
            FunctionCall.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                FunctionCallArgument,
                TokenUnits[TokenType.RightBracket],
            };
            FunctionCall.Execute = null;

            FunctionCallArgument.Name = "Function Call Argument";
            FunctionCallArgument.Type = ParseUnitType.SingleOptional;
            FunctionCallArgument.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgument.Children = new List<ParseUnit>()
            {
                FunctionCallArgumentItem,
                FunctionCallArgumentLoop
            };
            FunctionCallArgument.Execute = null;

            FunctionCallArgumentLoop.Name = "Function Call Argument Loop";
            FunctionCallArgumentLoop.Type = ParseUnitType.SingleOptional;
            FunctionCallArgumentLoop.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgumentLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                FunctionCallArgument,
            };
            FunctionCallArgument.Execute = null;

            FunctionCallArgumentItem.Name = "Function Call Argument Item";
            FunctionCallArgumentItem.Type = ParseUnitType.Single;
            FunctionCallArgumentItem.ChildType = ParseUnitChildType.AllChild;
            FunctionCallArgumentItem.Children = new List<ParseUnit>()
            {
                Expression,
            };
            FunctionCallArgumentItem.Execute = arg => arg.Instance.Children[0].Execute(arg);

            ArraySubscripting.Name = "Array Subscripting";
            ArraySubscripting.Type = ParseUnitType.Single;
            ArraySubscripting.ChildType = ParseUnitChildType.AllChild;
            ArraySubscripting.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftSquareBracket],
                Expression,
                TokenUnits[TokenType.RightSquareBracket],
            };
            //todo ArraySubscripting

            MemberAccess.Name = "Member Access";
            MemberAccess.Type = ParseUnitType.Single;
            MemberAccess.ChildType = ParseUnitChildType.AllChild;
            MemberAccess.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Dot],
                LeftValue,
            };
            //todo MemberAccess



            // level-3 (RTL): Unary plus and minus, Logical NOT and bitwise NOT
            ExpressionsHelper[3].Type = ParseUnitType.Single;
            ExpressionsHelper[3].Children = new List<ParseUnit>()
            {
                Operators[3],
                Expressions[3],
            };
            Expressions[3].ChildType = ParseUnitChildType.OneChild;
            Expressions[3].Children = new List<ParseUnit>()
            {
                ExpressionsHelper[3],
                Expressions[2],
            };
            Operators[3].Children = new List<ParseUnit>() { TokenUnits[TokenType.Plus], TokenUnits[TokenType.Minus], TokenUnits[TokenType.BooleanNot], TokenUnits[TokenType.BitwiseNot] };


            // level-4: not used
            Expressions[4].Children = new List<ParseUnit>() { Expressions[3] };


            // level-5: Multiplication, division, remainder
            //
            // note: eliminate left recursion
            // Example:
            //
            // A -> Aα | β
            // 
            // will be converted to
            // 
            // A -> βR
            // R -> αR | ε
            // (where ε stands for null)
            //
            // In this situation, A is Expression[5], α is Operators[5] and Expressions[4], β is Expressions[4],
            // R is ExpressionHelper[5]
            {
                const int i = 5;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.Times], TokenUnits[TokenType.Divide], TokenUnits[TokenType.Mod] };
            }

            // level-6: plus, minus
            // note: eliminate left recursion
            {
                const int i = 6;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.Plus], TokenUnits[TokenType.Minus] };
            }


            // level-7:  Bitwise left shift and right shift 
            // note: eliminate left recursion
            {
                const int i = 7;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseLeftShiftUnsigned], TokenUnits[TokenType.BitwiseRightShiftUnsigned], TokenUnits[TokenType.BitwiseLeftShiftSigned], TokenUnits[TokenType.BitwiseRightShiftSigned] };
            }


            // level-8: not used
            Expressions[8].Children = new List<ParseUnit>() { Expressions[7] };


            // level-9: <   <= 	>   >=
            {
                const int i = 9;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.LessThan], TokenUnits[TokenType.LessEqualThan], TokenUnits[TokenType.GreaterThan], TokenUnits[TokenType.GreaterEqualThan] };
            }


            // level-10:  ==   != 
            {
                const int i = 10;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.EqualTo], TokenUnits[TokenType.NotEqualTo] };
            }


            // level-11: Bitwise AND
            {
                const int i = 11;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseAnd] };
            }


            // level-12: Bitwise XOR
            {
                const int i = 12;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseXor] };
            }


            // level-13: Bitwise OR 
            {
                const int i = 13;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BitwiseOr] };
            }


            // level-14: Logical AND
            {
                const int i = 14;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanAnd] };
            }


            // level-15 (not existed on C/C++): Logical XOR
            {
                const int i = 15;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanXor] };
            }


            // level-16 (corresponding 15): Logical OR
            {
                const int i = 16;
                Expressions[i].Children = new List<ParseUnit>() { Expressions[i - 1], ExpressionsHelper[i] };
                ExpressionsHelper[i].Children = new List<ParseUnit>() { Operators[i], Expressions[i - 1], ExpressionsHelper[i] };

                Operators[i].Children = new List<ParseUnit>() { TokenUnits[TokenType.BooleanOr] };
            }


            // level-17 (RTL) (corresponding 16): assign =
            ExpressionsHelper[17].Type = ParseUnitType.Single;
            ExpressionsHelper[17].ChildType = ParseUnitChildType.AllChild;
            ExpressionsHelper[17].Children = new List<ParseUnit>()
            {
                LeftValue,
                Operators[17],
                Expressions[17]
            };
            ExpressionsHelper[17].Execute = arg =>
            {
                var leftVexp =

            };

            Expressions[17].ChildType = ParseUnitChildType.OneChild;
            Expressions[17].Children = new List<ParseUnit>()
            {
                ExpressionsHelper[17],
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
