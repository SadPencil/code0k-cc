using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code0k_cc
{
    static class SymbolAnalyzer
    {
        private delegate RequireDelegateResult RequireDelegate(SymbolTable table, int tableIndex);

        struct RequireDelegateResult
        {
            public bool Result;
            public ParserTable.Item Item;
            public int SymbolTableIndex;
        }

        private readonly static RequireDelegateResult BadResult = new RequireDelegateResult()
        {
            Result = false,
            Item = null,
            SymbolTableIndex = -1
        };

        private static RequireDelegateResult Require(List<RequireDelegate> singleList, string type, SymbolTable table, int tableIndex)
        {
            RequireDelegateResult ret = new RequireDelegateResult()
            {
                Result = true,
                Item = new ParserTable.Item() { Type = type },
                SymbolTableIndex = tableIndex
            };

            foreach (var d in singleList)
            {
                var dret = d(table, ret.SymbolTableIndex);
                if (!dret.Result)
                {
                    return BadResult;
                }
                ret.Item.Items.Add(dret.Item);
                ret.SymbolTableIndex = dret.SymbolTableIndex;
            }

            return ret;
        }
        private static RequireDelegateResult Require(List<List<RequireDelegate>> lists, string type, SymbolTable table, int tableIndex)
        {
            foreach (var list in lists)
            {
                var ret = Require(list, type, table, tableIndex);
                if (ret.Result)
                {
                    return ret;
                }
            }

            return BadResult;
        }

        private readonly static Dictionary<TokenType, RequireDelegate> SymbolDelegates;


        static SymbolAnalyzer()
        {
            // symbol
            SymbolDelegates = new Dictionary<TokenType, RequireDelegate>();
            foreach (var symbol in TokenType.GetAll())
            {
                SymbolDelegates.Add(
                    symbol,
                    (SymbolTable table, int tableIndex) =>
                    {
                        return RequireSymbol(symbol, table, tableIndex);
                    });
            }

        }

        private static RequireDelegateResult RequireSymbol(TokenType symbol, SymbolTable table, int tableIndex)
        {
            var next = table.List[tableIndex];
            if (symbol == next.Symbol)
            {
                var item = new ParserTable.Item
                {
                    Type = next.Symbol.Type.ToString(),
                    Value = next.Name
                };

                return new RequireDelegateResult()
                {
                    Result = true,
                    Item = item,
                    SymbolTableIndex = tableIndex + 1
                };
            }
            else
            {
                return BadResult;
            }

        }

        private static RequireDelegateResult RequireMainProgram(SymbolTable table, int tableIndex) => Require(new List<RequireDelegate>()
        {
            RequireSubProgram,
            SymbolDelegates[TokenType.Dot]
        }, "MainProgram", table, tableIndex);

        private static RequireDelegateResult RequireSubProgram(SymbolTable table, int tableIndex) => Require(new List<List<RequireDelegate>>()
        {
            new List<RequireDelegate>(){ RequireConstDeclaration }
            new List<RequireDelegate>(){ RequireVarDeclaration }
            new List<RequireDelegate>(){ RequireProcedureDeclaration }
            new List<RequireDelegate>(){ RequireStatement }
        }, "SubProgram", table, tableIndex);
         

        private ParserTable.Item RequireSubProgram()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "SubProgram" };
            try
            {
                try { item.Items.Add(this.RequireConstDeclaration()); } catch (ParseException) { }
                try { item.Items.Add(this.RequireVarDeclaration()); } catch (ParseException) { }
                try { item.Items.Add(this.RequireProcedureDeclaration()); } catch (ParseException) { }
                item.Items.Add(this.RequireStatement());
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireConstDeclaration()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "ConstDeclaration" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Const));
                item.Items.Add(this.RequireConstDefinition());
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireSymbol(TokenType.Comma);
                        var item2 = this.RequireConstDefinition();
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;
                        break;
                    }
                }
                item.Items.Add(this.RequireSymbol(TokenType.Semicolon));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireConstDefinition()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "ConstDefinition" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                item.Items.Add(this.RequireSymbol(TokenType.EqualTo));
                item.Items.Add(this.RequireSymbol(TokenType.Number));

                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireVarDeclaration()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "VarDeclaration" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Var));
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireSymbol(TokenType.Comma);
                        var item2 = this.RequireSymbol(TokenType.Identifier);
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;

                        break;
                    }
                }
                item.Items.Add(this.RequireSymbol(TokenType.Semicolon));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }

        }
        private ParserTable.Item RequireProcedureDeclaration()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "ProcedureDeclaration" };
            try
            {
                item.Items.Add(this.RequireProcedureHeader());
                item.Items.Add(this.RequireSubProgram());
                item.Items.Add(this.RequireSymbol(TokenType.Semicolon));
                try
                {
                    item.Items.Add(this.RequireProcedureDeclaration());
                }
                catch (ParseException)
                {

                }
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireProcedureHeader()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "ProcedureHeader" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Procedure));
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                item.Items.Add(this.RequireSymbol(TokenType.Semicolon));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireStatement()
        {
            try
            {
                return this.RequireAssignStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireIfStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireWhileStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireCallStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireReadStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireWriteStatement();

            }
            catch (ParseException) { }

            try
            {
                return this.RequireCompoundStatement();

            }
            catch (ParseException) { }

            return new ParserTable.Item() { Type = "NullStatement" };
            //allow null
        }

        private ParserTable.Item RequireAssignStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "AssignStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                item.Items.Add(this.RequireSymbol(TokenType.Assign));
                item.Items.Add(this.RequireExpression());
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireCompoundStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "CompoundStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Begin));
                item.Items.Add(this.RequireStatement());
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireSymbol(TokenType.Semicolon);
                        var item2 = this.RequireStatement();
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;
                        break;
                    }
                }

                item.Items.Add(this.RequireSymbol(TokenType.End));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }

        }
        private ParserTable.Item RequireCondition()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "Condition" };
            try
            {
                var anotherBackup = this.SymbolTableIndex;
                try
                {
                    var item1 = this.RequireSymbol(TokenType.Odd);
                    var item2 = this.RequireExpression();
                    item.Items.Add(item1);
                    item.Items.Add(item2);
                    return item;
                }
                catch (ParseException)
                {
                    this.SymbolTableIndex = anotherBackup;
                    item.Items.Add(this.RequireExpression());
                    item.Items.Add(this.RequireRelationships());
                    item.Items.Add(this.RequireExpression());
                    return item;
                }
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireExpression()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "Expression" };
            try
            {

                bool isFirstTime = true;
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {

                        try
                        {
                            item.Items.Add(this.RequireSymbol(TokenType.Plus));
                        }
                        catch (ParseException)
                        {
                            try
                            {
                                item.Items.Add(this.RequireSymbol(TokenType.Minus));
                            }
                            catch (ParseException e)
                            {
                                if (!isFirstTime)
                                {
                                    throw e;
                                }
                            }
                        }
                        item.Items.Add(this.RequireItem());
                        isFirstTime = false;
                    }
                    catch (ParseException e)
                    {
                        if (isFirstTime)
                        {
                            throw e;
                        }
                        else
                        {
                            this.SymbolTableIndex = anotherBackup;
                            break;
                        }
                    }
                }
                return item;
            }

            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireItem()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "Item" };
            try
            {
                item.Items.Add(this.RequireFactor());
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireTimesOrDivide();
                        var item2 = this.RequireFactor();
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;
                        break;
                    }
                }
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireFactor()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "Factor" };
            try
            {
                try
                {
                    item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                }
                catch (ParseException)
                {
                    try
                    {
                        item.Items.Add(this.RequireSymbol(TokenType.Number));
                    }
                    catch (ParseException)
                    {
                        item.Items.Add(this.RequireSymbol(TokenType.LeftBracket));
                        item.Items.Add(this.RequireExpression());
                        item.Items.Add(this.RequireSymbol(TokenType.RightBracket));
                    }
                }
                return item;

            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequirePlusOrMinus()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "PlusOrMinus" };
            try
            {
                try
                {
                    item.Items.Add(this.RequireSymbol(TokenType.Plus));
                }
                catch (ParseException)
                {
                    item.Items.Add(this.RequireSymbol(TokenType.Minus));
                }
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireTimesOrDivide()
        {
            var backup = this.SymbolTableIndex;

            var item = new ParserTable.Item() { Type = "TimesOrDivide" };
            try
            {
                try
                {
                    item.Items.Add(this.RequireSymbol(TokenType.Times));
                }
                catch (ParseException)
                {
                    item.Items.Add(this.RequireSymbol(TokenType.Divide));
                }
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireRelationships()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "Relationships" };
            try
            {
                try
                {
                    return this.RequireSymbol(TokenType.EqualTo);

                }
                catch (ParseException) { }

                try
                {
                    return this.RequireSymbol(TokenType.NotEqualTo);

                }
                catch (ParseException) { }

                try
                {
                    return this.RequireSymbol(TokenType.LessThan);

                }
                catch (ParseException) { }

                try
                {
                    return this.RequireSymbol(TokenType.GreaterThan);

                }
                catch (ParseException) { }

                try
                {
                    return this.RequireSymbol(TokenType.LessEqualThan);

                }
                catch (ParseException) { }

                try
                {
                    return this.RequireSymbol(TokenType.GreaterEqualThan);

                }
                catch (ParseException) { }

                throw new ParseException("Releationship not found.");
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireIfStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "IfStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.If));
                item.Items.Add(this.RequireCondition());
                item.Items.Add(this.RequireSymbol(TokenType.Then));
                item.Items.Add(this.RequireStatement());
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireCallStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "CallStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Call));
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireWhileStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "WhileStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.While));
                item.Items.Add(this.RequireCondition());
                item.Items.Add(this.RequireSymbol(TokenType.Do));
                item.Items.Add(this.RequireStatement());
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
        private ParserTable.Item RequireReadStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "ReadStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Read));
                item.Items.Add(this.RequireSymbol(TokenType.LeftBracket));
                item.Items.Add(this.RequireSymbol(TokenType.Identifier));
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireSymbol(TokenType.Comma);
                        var item2 = this.RequireSymbol(TokenType.Identifier);
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;
                        break;
                    }
                }
                item.Items.Add(this.RequireSymbol(TokenType.RightBracket));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }

        private ParserTable.Item RequireWriteStatement()
        {
            var backup = this.SymbolTableIndex;
            var item = new ParserTable.Item() { Type = "WriteStatement" };
            try
            {
                item.Items.Add(this.RequireSymbol(TokenType.Write));
                item.Items.Add(this.RequireSymbol(TokenType.LeftBracket));
                item.Items.Add(this.RequireExpression());
                while (true)
                {
                    var anotherBackup = this.SymbolTableIndex;
                    try
                    {
                        var item1 = this.RequireSymbol(TokenType.Comma);
                        var item2 = this.RequireExpression();
                        item.Items.Add(item1);
                        item.Items.Add(item2);
                    }
                    catch (ParseException)
                    {
                        this.SymbolTableIndex = anotherBackup;
                        break;
                    }
                }

                item.Items.Add(this.RequireSymbol(TokenType.RightBracket));
                return item;
            }
            catch (ParseException e)
            {
                this.SymbolTableIndex = backup;
                throw e;
            }
        }
    }
}
