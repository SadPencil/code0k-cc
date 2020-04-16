﻿using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class Parser
    {
        private static ParseUnit ParseUnit { get; } = GetRootParseUnit();


        private static ParseUnit GetRootParseUnit()
        {
            Dictionary<TokenType, ParseUnit> TokenUnits = new Dictionary<TokenType, ParseUnit>();
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

            ParseUnit MainProgram = new ParseUnit();
            ParseUnit FunctionDeclaration = new ParseUnit();
            ParseUnit FunctionImplementation = new ParseUnit();

            ParseUnit FunctionDeclarationArguments = new ParseUnit();
            ParseUnit FunctionArgumentUnit = new ParseUnit();
            ParseUnit FunctionArgumentLastUnit = new ParseUnit();

            ParseUnit FunctionCall = new ParseUnit();

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

            ParseUnit ArrayGetter = new ParseUnit();
            ParseUnit Property = new ParseUnit();
            ParseUnit UnaryOperator = new ParseUnit();
            ParseUnit BinaryOperator = new ParseUnit();
            ParseUnit Expression = new ParseUnit();
            ParseUnit UnaryExpression = new ParseUnit();
            ParseUnit BinaryExpression = new ParseUnit();

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
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End]
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

            FunctionCall.Name = "Function Call";
            FunctionCall.Type = ParseUnitType.Single;
            FunctionCall.ChildType = ParseUnitChildType.AllChild;
            FunctionCall.Children = new List<ParseUnit>()
            {
                LeftValue,
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
                //TokenUnits[TokenType.Var],
                TokenUnits[TokenType.Const]
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
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End],
                OptionalElseStatement
            };

            OptionalElseStatement.Name = "Else Statement";
            OptionalElseStatement.Type = ParseUnitType.SingleOptional;
            OptionalElseStatement.ChildType = ParseUnitChildType.AllChild;
            OptionalElseStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Else],
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End],
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
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End],
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
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End],
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
                        Property,
                        ArrayGetter
                    }
                }
            };

            ArrayGetter.Name = "Array Getter";
            ArrayGetter.Type = ParseUnitType.Single;
            ArrayGetter.ChildType = ParseUnitChildType.AllChild;
            ArrayGetter.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftSquareBracket],
                RightValue,
                TokenUnits[TokenType.RightSquareBracket]
            };

            Property.Name = "Property";
            Property.Type = ParseUnitType.Single;
            Property.ChildType = ParseUnitChildType.AllChild;
            Property.Children = new List<ParseUnit>()
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

            Expression.Name = "Expression";
            Expression.Type = ParseUnitType.SingleOptional;
            Expression.ChildType = ParseUnitChildType.FirstChild;
            Expression.Children = new List<ParseUnit>()
            {
                BinaryExpression,
                UnaryExpression,
                FunctionCall,
                TokenUnits[TokenType.Number],
                LeftValue
            };

            BinaryExpression.Name = "Binary Expression";
            BinaryExpression.Type = ParseUnitType.Single;
            BinaryExpression.ChildType = ParseUnitChildType.AllChild;
            BinaryExpression.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                Expression,
                BinaryOperator,
                Expression,
                TokenUnits[TokenType.RightBracket]
            };

            UnaryExpression.Name = "Unary Expression";
            UnaryExpression.Type = ParseUnitType.Single;
            UnaryExpression.ChildType = ParseUnitChildType.AllChild;
            UnaryExpression.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.LeftBracket],
                UnaryOperator,
                Expression,
                TokenUnits[TokenType.RightBracket]
            };

            BinaryOperator.Name = "Binary Operator";
            BinaryOperator.Type = ParseUnitType.Single;
            BinaryOperator.ChildType = ParseUnitChildType.FirstChild;

            //todo

            return MainProgram;

        }

    }
}
