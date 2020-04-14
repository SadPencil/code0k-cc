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

            ParseUnit AssignStatement = new ParseUnit();
            ParseUnit LeftValue = new ParseUnit();
            ParseUnit AssignMark = new ParseUnit();
            ParseUnit RightValue = new ParseUnit();

            ParseUnit DefinitionStatement = new ParseUnit();
            ParseUnit CallStatement = new ParseUnit();
            ParseUnit IfStatement = new ParseUnit();
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
            //todo


            return MainProgram;

        }

    }
}
