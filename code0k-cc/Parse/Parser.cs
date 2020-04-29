﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using code0k_cc.Lex;
using code0k_cc.Runtime;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.ExeArg;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Nizk;
using code0k_cc.Runtime.ValueOfType;

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
                        Depth = depth,
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
                            Depth = depth,
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
                            Depth = depth,
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
                            Depth = depth,
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
                        Depth = depth,
                        Row = newRowCol.Row,
                        Column = newRowCol.Column,
                    };
                    return ret;
                }

            }
            else if (unit.ChildType == ParseUnitChildType.OneChild)
            {
                ParseResult goodResult = null;
                ParseResult badResult = null;
                int badResultDepth = 0;
                foreach (var unitChild in unit.Children)
                {
                    // if all results are bad results, and unit.Type == ParseUnitType.Single, return the deepest bad result instead of the last one
                    var ret = Parse(unitChild, tokenList, pos, depth + 1);
                    if (ret.Success)
                    {
                        goodResult = ret;
                        break;
                    }

                    if (ret.Depth >= badResultDepth)
                    {
                        badResultDepth = ret.Depth;
                        badResult = ret;
                    }
                    else
                    {
                        Debug.Assert(badResult != null);
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
                            Depth = depth,
                            Row = token.Row,
                            Column = token.Column,
                        };
                        return ret;
                    }
                    else if (unit.Type == ParseUnitType.Single)
                    {
                        // failed
                        Debug.Assert(badResult != null);
                        return badResult;
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
                        Depth = goodResult.Depth,
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
            ParseUnit GlobalFunctionDeclarationStatement = new ParseUnit();

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
            //ParseUnit ForStatement = new ParseUnit();
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
            //todo
            MainProgram.Execute = arg => throw new NotImplementedException();

            // MainProgram.Execute = arg =>
            //{
            //    //block: provide built-in types

            //    //todo: redundancy code here.

            //    // prepare environment
            //    var mainBlock = new BasicBlock(arg.Block.Block);

            //    var mainBlockOverlay = new OverlayBlock(arg.Block.Overlay, mainBlock);

            //    foreach (var instanceChild in arg.Instance.Children)
            //    {
            //        _ = instanceChild?.Execute(new ExeArg() { Block = mainBlockOverlay });
            //    }

            //    // find & execute "main"
            //    var mainFunc = (FunctionDeclarationValue) mainBlockOverlay.GetVariableRefRef("main", false, true).VariableRef.Variable.Value;

            //    if (mainFunc.Instance == null)
            //    {
            //        throw new Exception($"Unimplemented function \"{mainFunc.FunctionName}\"");
            //    }

            //    // create new block
            //    var newFuncBlock = new BasicBlock(mainBlock);
            //    var newFuncBlockOverlay = new OverlayBlock(arg.Block.Overlay, newFuncBlock);
            //    // execute function
            //    var funRet = mainFunc.Instance.Execute(new ExeArg() { Block = newFuncBlockOverlay }).StatementResult;
            //    return funRet.Type switch
            //    {
            //        StatementResultType.Return => new ExeResult()
            //        {
            //            StatementResult = new StatementResult()
            //            { ReturnVariableRefRef = funRet.ReturnVariableRefRef, Type = StatementResultType.Return }
            //        },
            //        StatementResultType.Normal => new ExeResult()
            //        {
            //            StatementResult = new StatementResult()
            //            { ReturnVariableRefRef = new VariableRefRef(new VariableRef() { Variable = NType.Void.NewValue() }), Type = StatementResultType.Return }
            //        },
            //        StatementResultType.Break => throw new Exception($"Unexpected \"break\" without loop statement while executing function \"{mainFunc.FunctionName}\""),
            //        StatementResultType.Continue => throw new Exception($"Unexpected \"continue\" without loop statement while executing function \"{mainFunc.FunctionName}\""),
            //        _ => throw new Exception("Assert failed!"),
            //    };
            //};

            MainProgramItem.Name = "Main Program Item";
            MainProgramItem.Type = ParseUnitType.Single;
            MainProgramItem.ChildType = ParseUnitChildType.OneChild;
            MainProgramItem.Children = new List<ParseUnit>()
            {
                GlobalDefinitionStatement,
                GlobalFunctionDeclarationStatement,
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
                TokenUnits[TokenType.Max],
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket],
            };
            FunctionDeclaration.Execute = arg =>
            {
                var typeUnitResult = arg.Instance.Children[0].Execute(arg).TypeUnitResult;
                var funRetNType = NType.GetNType(typeUnitResult);

                var funName = arg.Instance.Children[1].Token.Value;

                var funArgs = arg.Instance.Children[3]?.Execute(arg)?.FunctionDeclarationValue.Arguments;

                var maxIntVar = arg.Instance.Children[7].Execute(arg)?.ExpressionResult.VariableRefRef.VariableRef.Variable;

                var maxInt = ( (NizkUInt32Value) NType.UInt32.Assign(maxIntVar, NType.UInt32).Value ).Value;

                var funT = new FunctionDeclarationValue() { Arguments = funArgs, FunctionName = funName, ReturnType = funRetNType, MaxLoop = maxInt, Instance = null };

                arg.Block.AddVariable(funName, new Variable() { Type = NType.Function, Value = funT }, true);

                return new ExeResult() { FunctionDeclarationValue = funT };
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
                var functionName = arg.Instance.Children[0].Execute(arg).FunctionDeclarationValue.FunctionName;
                // 2. set the FunctionDeclarationValue.Instance to the compound statement
                var funT = (FunctionDeclarationValue) arg.Block.GetVariableRefRef(functionName, false, false).VariableRef.Variable.Value;
                funT.Instance = arg.Instance.Children[1];

                return new ExeResult() { FunctionDeclarationValue = funT };
            };


            FunctionDeclarationArguments.Name = "Function Declaration Arguments";
            FunctionDeclarationArguments.Type = ParseUnitType.SingleOptional;
            FunctionDeclarationArguments.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArguments.Children = new List<ParseUnit>()
            {
                FunctionDeclarationArgumentUnit,
                FunctionDeclarationArgumentLoop,
            };
            FunctionDeclarationArguments.Execute = arg =>
            {
                var arguments = new List<(string VarName, NType Type)>();
                var unitIns = arg.Instance.Children[0];
                var loopIns = arg.Instance.Children[1];
                while (true)
                {
                    var typeResult = unitIns.Children[0].Execute(arg).TypeUnitResult;
                    var argNType = NType.GetNType(typeResult);
                    var argName = unitIns.Children[1].Token.Value;
                    arguments.Add((VarName: argName, Type: argNType));

                    if (loopIns == null) break;
                    unitIns = loopIns.Children[1];
                    loopIns = loopIns.Children[2];
                }

                return new ExeResult() { FunctionDeclarationValue = new FunctionDeclarationValue() { Arguments = arguments } };
            };

            FunctionDeclarationArgumentUnit.Name = "Function Declaration Argument Unit";
            FunctionDeclarationArgumentUnit.Type = ParseUnitType.Single;
            FunctionDeclarationArgumentUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArgumentUnit.Children = new List<ParseUnit>()
            {
                TypeUnit,
                TokenUnits[TokenType.Identifier]
            };
            FunctionDeclarationArgumentUnit.Execute = null;

            FunctionDeclarationArgumentLoop.Name = "Function Declaration Argument Loop";
            FunctionDeclarationArgumentLoop.Type = ParseUnitType.SingleOptional;
            FunctionDeclarationArgumentLoop.ChildType = ParseUnitChildType.AllChild;
            FunctionDeclarationArgumentLoop.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Comma],
                FunctionDeclarationArgumentUnit,
                FunctionDeclarationArgumentLoop,
            };
            FunctionDeclarationArgumentLoop.Execute = null;


            StatementBody.Name = "Statement Body";
            StatementBody.Type = ParseUnitType.SingleOptional;
            StatementBody.ChildType = ParseUnitChildType.AllChild;
            StatementBody.Children = new List<ParseUnit>()
            {
                Statement,
                StatementBody
            };
            StatementBody.Execute = arg =>
            {
                // note: adding block level is done at CompoundStatement instead of here
                var stmtRawRet = arg.Instance.Children[0].Execute(arg).StatementResult;
                var nextStmtInstance = arg.Instance.Children[1]?.Children[0];
                switch (stmtRawRet)
                {
                    case StatementResultOneCase stmtRet:
                        Debug.Assert(stmtRet.Overlay == arg.Block.Overlay);
                        switch (stmtRet.ExecutionResultType)
                        {
                            case StatementResultType.Continue:
                            case StatementResultType.Break:
                            case StatementResultType.Return:
                                return new ExeResult() { StatementResult = stmtRet };
                            case StatementResultType.Normal:
                                // execute the rest statements 
                                if (nextStmtInstance == null)
                                {
                                    // great, normal exit. return as-is
                                    return new ExeResult() { StatementResult = stmtRet };
                                }
                                else
                                {
                                    // execute next
                                    return nextStmtInstance.Execute(arg);
                                }
                            default:
                                throw new Exception("Assert failed!");
                        }
                    case StatementResultTwoCase stmtRet:
                        if (nextStmtInstance == null)
                        {
                            // okay, return as-is
                            return new ExeResult() { StatementResult = stmtRet };
                        }
                        else
                        {
                            // let the rest statements follow the condition where resultType == normal
                            // todo: this task can be done in parallel
                            var queue = new List<(StatementResult ItemRet, StatementResultTwoCase ItemParentRet, bool IsParentTrueCase)>()
                            {
                                ( ItemRet: stmtRet.TrueCase,ItemParentRet:stmtRet,IsParentTrueCase: true),
                                ( ItemRet: stmtRet.FalseCase,ItemParentRet:stmtRet,IsParentTrueCase:false),
                            };

                            while (queue.Count > 0)
                            {
                                var (itemRet, itemParentRet, isParentTrueCase) = queue[0];
                                queue.RemoveAt(0);

                                switch (itemRet)
                                {
                                    case StatementResultTwoCase item:
                                        queue.Add((ItemRet: item.TrueCase, ItemParentRet: item, IsParentTrueCase: true));
                                        queue.Add((ItemRet: item.FalseCase, ItemParentRet: item, IsParentTrueCase: false));
                                        break;

                                    case StatementResultOneCase item:
                                        // needs more assert here to validate the tree structure. omit it
                                        Debug.Assert(item.Overlay != arg.Block.Overlay);
                                        switch (item.ExecutionResultType)
                                        {
                                            case StatementResultType.Continue:
                                            case StatementResultType.Break:
                                            case StatementResultType.Return:
                                                break;
                                            case StatementResultType.Normal:
                                                // execute next statement at this overlay
                                                var retRaw = nextStmtInstance.Execute(new ExeArg()
                                                {
                                                    Block = new OverlayBlock(item.Overlay, arg.Block.Block)
                                                }).StatementResult;

                                                //save retRaw to the parent
                                                if (isParentTrueCase)
                                                {
                                                    itemParentRet.TrueCase = retRaw;
                                                }
                                                else
                                                {
                                                    itemParentRet.FalseCase = retRaw;
                                                }

                                                //todo: do an optimization described below

                                                break;
                                            default:
                                                throw new Exception("Assert failed!");

                                        }
                                        break;
                                    default:
                                        throw new Exception("Assert failed!");
                                }

                            }

                            //todo: optimization: if all the retRaw are all Break or all Continue, combining overlay can be done here

                            // now return the result
                            return new ExeResult() { StatementResult = stmtRet };
                        }
                    default:
                        throw new Exception("Assert failed!");

                }
            };

            StatementSemicolon.Name = "Statement Semicolon";
            StatementSemicolon.Type = ParseUnitType.Single;
            StatementSemicolon.ChildType = ParseUnitChildType.AllChild;
            StatementSemicolon.Children = new List<ParseUnit>()
            {
                StatementSemicolonCollection,
                TokenUnits[TokenType.Semicolon],
            };
            StatementSemicolon.Execute = arg => arg.Instance.Children[0].Execute(arg);

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
            StatementSemicolonCollection.Execute = arg =>
             {
                 if (arg.Instance.Children[0].ParseUnit == Expression)
                 {
                     _ = arg.Instance.Children[0].Execute(arg);

                     return new ExeResult()
                     {
                         StatementResult = new StatementResultOneCase()
                         {
                             Overlay = arg.Block.Overlay,
                             ExecutionResultType = StatementResultType.Normal,
                         }
                     };
                 }
                 else
                 {
                     return arg.Instance.Children[0].Execute(arg);
                 }
             };


            Statement.Name = "Statement";
            Statement.Type = ParseUnitType.Single;
            Statement.ChildType = ParseUnitChildType.OneChild;
            Statement.Children = new List<ParseUnit>()
            {
                CompoundStatement,
                IfStatement,
               // ForStatement,
                WhileStatement,
                StatementSemicolon,
            };
            Statement.Execute = arg => arg.Instance.Children[0].Execute(arg);

            GlobalDefinitionStatement.Name = "Global Definition Statement";
            GlobalDefinitionStatement.Type = ParseUnitType.Single;
            GlobalDefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            GlobalDefinitionStatement.Children = new List<ParseUnit>()
            {
                DefinitionStatement,
                TokenUnits[TokenType.Semicolon],
            };
            GlobalDefinitionStatement.Execute = arg => arg.Instance.Children[0].Execute(arg);

            GlobalFunctionDeclarationStatement.Name = "Global Function Declaration Statement";
            GlobalFunctionDeclarationStatement.Type = ParseUnitType.Single;
            GlobalFunctionDeclarationStatement.ChildType = ParseUnitChildType.AllChild;
            GlobalFunctionDeclarationStatement.Children = new List<ParseUnit>()
            {
                FunctionDeclaration,
                TokenUnits[TokenType.Semicolon],
            };
            GlobalFunctionDeclarationStatement.Execute = arg => arg.Instance.Children[0].Execute(arg);


            ReturnStatement.Name = "Return Statement";
            ReturnStatement.Type = ParseUnitType.Single;
            ReturnStatement.ChildType = ParseUnitChildType.AllChild;
            ReturnStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Return],
                Expression,
            };
            ReturnStatement.Execute = arg =>
            {
                var expRefRef = arg.Instance.Children[1].Execute(arg).ExpressionResult.VariableRefRef;
                return new ExeResult()
                {
                    StatementResult = new StatementResultOneCase()
                    {
                        Overlay = arg.Block.Overlay,
                        ExecutionResultType = StatementResultType.Return,
                        ReturnVariableRefRef = expRefRef,
                    }
                };
            };

            BreakStatement.Name = "Break Statement";
            BreakStatement.Type = ParseUnitType.Single;
            BreakStatement.ChildType = ParseUnitChildType.AllChild;
            BreakStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Break],
            };
            BreakStatement.Execute = arg => new ExeResult()
            {
                StatementResult = new StatementResultOneCase()
                {
                    Overlay = arg.Block.Overlay,
                    ExecutionResultType = StatementResultType.Break,
                }
            };

            ContinueStatement.Name = "Continue Statement";
            ContinueStatement.Type = ParseUnitType.Single;
            ContinueStatement.ChildType = ParseUnitChildType.AllChild;
            ContinueStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Continue],
            };
            ContinueStatement.Execute = arg => new ExeResult()
            {
                StatementResult = new StatementResultOneCase()
                {
                    Overlay = arg.Block.Overlay,
                    ExecutionResultType = StatementResultType.Continue,
                }
            };

            DefinitionStatement.Name = "Definition Statement";
            DefinitionStatement.Type = ParseUnitType.Single;
            DefinitionStatement.ChildType = ParseUnitChildType.AllChild;
            DefinitionStatement.Children = new List<ParseUnit>()
            {
                TypeUnit,
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Assign],
                Expression
            };
            DefinitionStatement.Execute = arg =>
            {
                var typeR = arg.Instance.Children[0].Execute(arg).TypeUnitResult;
                var ntype = NType.GetNType(typeR);
                var varName = arg.Instance.Children[1].Token.Value;

                var expRefRef = arg.Instance.Children[3].Execute(arg).ExpressionResult.VariableRefRef;

                var newExp = expRefRef.VariableRef.Variable.Assign(ntype);

                arg.Block.AddVariable(varName, newExp, false);

                return new ExeResult()
                {
                    StatementResult = new StatementResultOneCase()
                    {
                        Overlay = arg.Block.Overlay,
                        ExecutionResultType = StatementResultType.Normal,
                    }
                };

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
            IfStatement.Execute = arg =>
            {
                // first : judge whether the expression is nizk node
                var conditionVar = arg.Instance.Children[2].Execute(arg).ExpressionResult.VariableRefRef.VariableRef.Variable;
                if (conditionVar.Type != NType.Bool)
                {
                    //try implicit convert to bool type
                    conditionVar = conditionVar.Assign(NType.Bool);
                }

                if (conditionVar.Value.IsConstant)
                {
                    // normal if-statement  
                    //todo

                    //todo
                    throw new NotImplementedException();
                }
                else
                {
                    // if it is nizk node, take control of assignment/declaration statement
                    // record which variables have changed, and performs actions
                    // rollback to execute else statement
                    // in the end, combine the two results
                    var currentOverlay = arg.Block.Overlay;
                    var trueOverlay = new Overlay(currentOverlay);
                    var falseOverlay = new Overlay(currentOverlay);
                    var trueOverlayBlock = new OverlayBlock(trueOverlay, arg.Block.Block);
                    var falseOverlayBlock = new OverlayBlock(falseOverlay, arg.Block.Block);

                    var trueRetRaw = arg.Instance.Children[4].Execute(new ExeArg() { Block = trueOverlayBlock }).StatementResult;
                    var falseRetRaw = arg.Instance.Children[5].Execute(new ExeArg() { Block = falseOverlayBlock }).StatementResult;

                    //todo: optimization: if all the retRaw are all Break or all Continue or all normal, combining overlay can be done here

                    return new ExeResult()
                    {
                        StatementResult = new StatementResultTwoCase()
                        {
                            Condition = conditionVar,
                            FalseCase = falseRetRaw,
                            TrueCase = trueRetRaw,
                        }
                    };
                }

            };

            OptionalElseStatement.Name = "Else Statement";
            OptionalElseStatement.Type = ParseUnitType.SingleOptional;
            OptionalElseStatement.ChildType = ParseUnitChildType.AllChild;
            OptionalElseStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Else],
                CompoundStatement
            };
            OptionalElseStatement.Execute = arg => arg.Instance.Children[1].Execute(arg);

            //ForStatement.Name = "For Statement";
            //ForStatement.Type = ParseUnitType.Single;
            //ForStatement.ChildType = ParseUnitChildType.AllChild;
            //ForStatement.Children = new List<ParseUnit>()
            //{
            //    //todo change for statement parse unit
            //    // for i = a to/downto b do
            //    // where b-a (a-b for downto) must be a constant type and must be an integer with positive/zero value
            //    TokenUnits[TokenType.For],
            //    TokenUnits[TokenType.LeftBracket],
            //    Expression,
            //    TokenUnits[TokenType.Semicolon],
            //    Expression,
            //    TokenUnits[TokenType.Semicolon],
            //    Expression,
            //    TokenUnits[TokenType.RightBracket],
            //    TokenUnits[TokenType.Max],
            //    TokenUnits[TokenType.LeftBracket],
            //    Expression,
            //    TokenUnits[TokenType.RightBracket],
            //    CompoundStatement
            //};

            ////todo
            //ForStatement.Execute = arg => throw new NotImplementedException();

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
            WhileStatement.Execute = arg =>
            {
                // get max loop count
                var maxIntVar = arg.Instance.Children[6].Execute(arg).ExpressionResult.VariableRefRef.VariableRef.Variable;
                if (maxIntVar.Type != NType.Bool)
                {
                    //try implicit convert to bool type
                    maxIntVar = maxIntVar.Assign(NType.Bool);
                }

                if (!maxIntVar.Value.IsConstant)
                {
                    throw new Exception("Can't the determinate max loop count of where statement.");
                }

                UInt32 maxInt = ( (NizkUInt32Value) maxIntVar.Value ).Value;

                // dummy StatementResultTwoCase in order to achieve pointer to a StatementResult
                // do NOT return stmtRetPointer, be sure to return stmtRetPointer.TrueCase
                var stmtRetPointer = new StatementResultTwoCase()
                {
                    Condition = null, // unused
                    FalseCase = null, // unused
                    TrueCase = new StatementResultOneCase()
                    {
                        ExecutionResultType = StatementResultType.Normal,
                        Overlay = arg.Block.Overlay,
                    },
                };

                for (UInt32 i = 0; i < maxInt; ++i)
                {

                    // execute condition var and judge its content every time in the loop
                    var conditionVar = arg.Instance.Children[2].Execute(arg).ExpressionResult.VariableRefRef.VariableRef.Variable;
                    if (conditionVar.Type != NType.Bool)
                    {
                        //try implicit convert to bool type
                        conditionVar = conditionVar.Assign(NType.Bool);
                    }

                    if (conditionVar.Value.IsConstant)
                    {
                        if (!( ( (NizkBoolValue) conditionVar.Value ).Value ))
                        {
                            // stop the loop
                            break;
                        }
                    }

                    var queue = new List<(StatementResult ItemRet, StatementResultTwoCase ItemParentRet, bool IsParentTrueCase)>()
                    {
                        ( ItemRet: stmtRetPointer.TrueCase,ItemParentRet:stmtRetPointer,IsParentTrueCase: true),
                    };

                    while (queue.Count > 0)
                    {
                        var (itemRet, itemParentRet, isParentTrueCase) = queue[0];
                        queue.RemoveAt(0);
                        switch (itemRet)
                        {
                            case StatementResultTwoCase item:
                                queue.Add((ItemRet: item.TrueCase, ItemParentRet: item, IsParentTrueCase: true));
                                queue.Add((ItemRet: item.FalseCase, ItemParentRet: item, IsParentTrueCase: false));
                                break;

                            case StatementResultOneCase item:
                                switch (item.ExecutionResultType)
                                {
                                    case StatementResultType.Break:
                                    case StatementResultType.Return:
                                        break;

                                    // note that the behavior of continue is different from compound statement
                                    case StatementResultType.Continue:
                                    case StatementResultType.Normal:
                                        StatementResult retSave;
                                        if (conditionVar.Value.IsConstant)
                                        {
                                            Debug.Assert(( (NizkBoolValue) conditionVar.Value ).Value);
                                            retSave = arg.Instance.Children[8].Execute(new ExeArg()
                                            {
                                                Block = new OverlayBlock(item.Overlay, arg.Block.Block)
                                            }).StatementResult;
                                        }
                                        else
                                        {
                                            // while (condition) is just like while (true){if (condition) do_something; else break;} 

                                            retSave = new StatementResultTwoCase()
                                            {
                                                Condition = conditionVar,
                                                FalseCase = new StatementResultOneCase()
                                                {
                                                    ExecutionResultType = StatementResultType.Break,
                                                    Overlay = new Overlay(item.Overlay),
                                                },
                                                // execute
                                                TrueCase = arg.Instance.Children[8].Execute(new ExeArg()
                                                {
                                                    Block = new OverlayBlock(new Overlay(item.Overlay), arg.Block.Block)
                                                }).StatementResult,
                                            };

                                        }

                                        //save retSave to the parent
                                        if (isParentTrueCase)
                                        {
                                            itemParentRet.TrueCase = retSave;
                                        }
                                        else
                                        {
                                            itemParentRet.FalseCase = retSave;
                                        }

                                        break;
                                    default:
                                        throw new Exception("Assert failed!");
                                }

                                break;

                            default:
                                throw new Exception("Assert failed!");
                        }


                    }

                }

                // now change "continue" to "normal", but leave "break" and "return" alone
                {
                    var queue = new List<(StatementResult ItemRet, StatementResultTwoCase ItemParentRet, bool IsParentTrueCase)>()
                    {
                        (ItemRet: stmtRetPointer.TrueCase, ItemParentRet: stmtRetPointer, IsParentTrueCase: true),
                    };

                    while (queue.Count > 0)
                    {
                        var (itemRet, itemParentRet, isParentTrueCase) = queue[0];
                        queue.RemoveAt(0);
                        switch (itemRet)
                        {
                            case StatementResultTwoCase item:
                                queue.Add((ItemRet: item.TrueCase, ItemParentRet: item, IsParentTrueCase: true));
                                queue.Add((ItemRet: item.FalseCase, ItemParentRet: item, IsParentTrueCase: false));
                                break;

                            case StatementResultOneCase item:
                                if (item.ExecutionResultType == StatementResultType.Continue)
                                {
                                    item.ExecutionResultType = StatementResultType.Normal;
                                }
                                else if (item.ExecutionResultType == StatementResultType.Break ||
                                         item.ExecutionResultType == StatementResultType.Return ||
                                         item.ExecutionResultType == StatementResultType.Normal)
                                {
                                    // leave the ExecutionResultType alone
                                }
                                else
                                {
                                    throw new Exception("Assert failed!");
                                }

                                break;
                            default:
                                throw new Exception("Assert failed!");
                        }
                    }
                }

                //return ret
                return new ExeResult() { StatementResult = stmtRetPointer.TrueCase };

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
            CompoundStatement.Execute = arg =>
            {
                //todo new block
                return arg.Instance.Children[1].Execute(arg);
            };

            LeftValue.Name = "Left Value";
            LeftValue.Type = ParseUnitType.Single;
            LeftValue.ChildType = ParseUnitChildType.AllChild;
            LeftValue.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                LeftValueSuffixLoop,
            };
            LeftValue.Execute = arg =>
            {
                //todo LeftValueSuffixItem
                string varName = arg.Instance.Children[0].Token.Value;
                var varRefRef = arg.Block.GetVariableRefRef(varName, true, true);
                return new ExeResult() { ExpressionResult = new ExpressionResult() { VariableRefRef = varRefRef } };
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
            Expression.Execute = arg => new ExeResult() { ExpressionResult = arg.Instance.Children[0].Execute(arg).ExpressionResult };

            BracketExpression.Name = "Bracket Expression";
            BracketExpression.Type = ParseUnitType.Single;
            BracketExpression.ChildType = ParseUnitChildType.AllChild;
            BracketExpression.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                Expression,
                TokenUnits[TokenType.RightBracket]
            };
            BracketExpression.Execute = arg => new ExeResult() { ExpressionResult = arg.Instance.Children[1].Execute(arg).ExpressionResult };


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
                    string str = arg.Instance.Children[0].Token.Value;
                    return new ExeResult()
                    {
                        ExpressionResult = new ExpressionResult()
                        {
                            VariableRefRef = arg.Block.GetVariableRefRef(str, false, false)
                        }
                    };
                }
                else if (arg.Instance.Children[0].ParseUnit == TokenUnits[TokenType.Number])
                {
                    string str = arg.Instance.Children[0].Token.Value;
                    //todo: currently, only support UInt32, will add other number type later
                    var retVar = NType.UInt32.Parse(str);
                    return new ExeResult()
                    {
                        ExpressionResult = new ExpressionResult() { VariableRefRef = new VariableRefRef(new VariableRef() { Variable = retVar }) }
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

                    // let exp = op(exp) 

                    if (op.ParseUnit == FunctionCall)
                    {
                        // get func
                        var funcStruct = (FunctionDeclarationValue) exp.VariableRefRef.VariableRef.Variable.Value;

                        if (funcStruct.Instance == null)
                        {
                            throw new Exception($"Unimplemented function \"{funcStruct.FunctionName}\"");
                        }

                        // create new block
                        var newBlock = new BasicBlock(funcStruct.ParentBlock);
                        var newBlockOverlay = new OverlayBlock(arg.Block.Overlay, newBlock);

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
                            var newArgVal = argExp.VariableRefRef.VariableRef.Variable.Assign(argNType);

                            //add params
                            newBlockOverlay.AddVariable(argName, newArgVal, false);

                            ++argCount;
                        }

                        if (argCount != funcStruct.Arguments.Count)
                        {
                            throw new Exception($"Unexpected function arguments of function \"{funcStruct.FunctionName}\".");
                        }

                        // execute function

                        //todo
                        throw new NotImplementedException();

                        //var funRet = funcStruct.Instance.Execute(new ExeArg() { Block = newBlockOverlay }).StatementResult;
                        //switch (funRet.Type)
                        //{
                        //    case StatementResultType.Return:
                        //        exp = new ExpressionResult() { VariableRefRef = funRet.ReturnVariableRefRef };
                        //        break;
                        //    case StatementResultType.Normal:
                        //        return new ExeResult()
                        //        {
                        //            StatementResult = new StatementResult()
                        //            { ReturnVariableRefRef = new VariableRefRef(new VariableRef() { Variable = NType.Void.NewValue() }), Type = StatementResultType.Return }
                        //        };
                        //    case StatementResultType.Break:
                        //        throw new Exception($"Unexpected \"break\" without loop statement while executing function \"{funcStruct.FunctionName}\"");

                        //    case StatementResultType.Continue:
                        //        throw new Exception($"Unexpected \"continue\" without loop statement while executing function \"{funcStruct.FunctionName}\"");

                        //    default:
                        //        throw new Exception("Assert failed!");
                        //}


                    }
                    else if (op.ParseUnit == MemberAccess)
                    {
                        //todo
                        throw new NotImplementedException();
                    }
                    else if (op.ParseUnit == ArraySubscripting)
                    {
                        //todo
                        throw new NotImplementedException();
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
            ArraySubscripting.Execute = arg =>
            {
                //todo
                throw new NotImplementedException();
            };

            MemberAccess.Name = "Member Access";
            MemberAccess.Type = ParseUnitType.Single;
            MemberAccess.ChildType = ParseUnitChildType.AllChild;
            MemberAccess.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Dot],
                LeftValue,
            };
            MemberAccess.Execute = arg =>
            {
                //todo
                throw new NotImplementedException();
            };



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
                Debug.Assert(arg.Instance.Children[1].Token.TokenType == TokenType.Assign);

                var leftVarRefRef = arg.Instance.Children[0].Execute(arg).ExpressionResult.VariableRefRef;


                var rightExpRefRef = arg.Instance.Children[2].Execute(arg).ExpressionResult.VariableRefRef;

                //assign
                leftVarRefRef.VariableRef.Variable = rightExpRefRef.VariableRef.Variable.Assign(leftVarRefRef.VariableRef.Variable.Type);

                return new ExeResult() { ExpressionResult = new ExpressionResult() { VariableRefRef = leftVarRefRef } };
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
