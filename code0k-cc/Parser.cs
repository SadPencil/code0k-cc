using System;
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
            ParseUnit Function = new ParseUnit();

            ParseUnit FunctionArguments = new ParseUnit();
            ParseUnit FunctionArgumentUnit = new ParseUnit();
            ParseUnit FunctionArgumentLastUnit = new ParseUnit();

            ParseUnit StatementBody = new ParseUnit();
            ParseUnit Statement = new ParseUnit();

            ParseUnit Descriptions = new ParseUnit();
            ParseUnit AssignStatement = new ParseUnit();
            ParseUnit LeftValue = new ParseUnit();
            ParseUnit RightValue = new ParseUnit();

            ParseUnit DefinitionStatement = new ParseUnit();
            ParseUnit CallStatement = new ParseUnit();
            ParseUnit IfStatement = new ParseUnit();
            ParseUnit OptionalElseStatement = new ParseUnit();
            ParseUnit ForStatement = new ParseUnit();
            ParseUnit WhileStatement = new ParseUnit();
            ParseUnit CompoundStatement = new ParseUnit();

            ParseUnit NullUnit = new ParseUnit();


            // write the parse unit
            MainProgram.Name = "Main Program";
            MainProgram.Type = ParseUnitType.Multiple;
            MainProgram.ChildType = ParseUnitChildType.FirstChild;
            MainProgram.Children = new List<ParseUnit>()
            {
                Function,
                DefinitionStatement
            };

            Function.Name = "Function";
            Function.Type = ParseUnitType.Single;
            Function.ChildType = ParseUnitChildType.AllChild;
            Function.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionArguments,
                TokenUnits[TokenType.RightBracket],
                TokenUnits[TokenType.Begin],
                StatementBody,
                TokenUnits[TokenType.End]
            };

            FunctionArguments.Name = "Function Arguments";
            FunctionArguments.Type = ParseUnitType.SingleOptional;
            FunctionArguments.ChildType = ParseUnitChildType.AllChild;
            FunctionArguments.Children = new List<ParseUnit>()
            {
                FunctionArgumentUnit,
                FunctionArgumentLastUnit
            };

            FunctionArgumentUnit.Name = "Function Argument";
            FunctionArgumentUnit.Type = ParseUnitType.MultipleOptional;
            FunctionArgumentUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.Comma]
            };

            FunctionArgumentLastUnit.Name = "Function Argument";
            FunctionArgumentLastUnit.Type = ParseUnitType.Single;
            FunctionArgumentLastUnit.ChildType = ParseUnitChildType.AllChild;
            FunctionArgumentLastUnit.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.Identifier]
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
                TokenUnits[TokenType.Identifier],
                TokenUnits[TokenType.LeftBracket],
                FunctionArguments,
                TokenUnits[TokenType.RightBracket]
            };

            IfStatement.Name = "If Statement";
            IfStatement.Type = ParseUnitType.Single;
            IfStatement.ChildType = ParseUnitChildType.AllChild;
            IfStatement.Children = new List<ParseUnit>()
            {
                TokenUnits[TokenType.If],
                TokenUnits[TokenType.LeftBracket],
                RightValue,
                TokenUnits[TokenType.RightBracket],
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
                TokenUnits[TokenType.LeftBracket],
                RightValue,
                TokenUnits[TokenType.RightBracket],
                TokenUnits[TokenType.Max],
                TokenUnits[TokenType.LeftBracket],
                RightValue,
                TokenUnits[TokenType.RightBracket],
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

            //todo AssignStatement LeftValue RightValue


            return MainProgram;

        }

    }
}
