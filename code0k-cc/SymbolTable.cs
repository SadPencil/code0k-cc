using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace code0k_cc
{
    class SymbolTable
    {
        public class Item {
            public String Name;
            public Token Symbol;
            public Int32 Address;

            public Item(String name, Token symbol, Int32 address)
            {
                this.Name = name;
                this.Symbol = symbol;
                this.Address = address;
            }
        }
        public List<Item> List = new List<Item>();
        private Dictionary<string, Item> variables { get; } = new Dictionary<string, Item>();
        private int nextAddress = 0;
        private readonly List<Token> symbols = Token.GetAll();

        public SymbolTable() { }

        public bool TryAppend(String word)
        {
            foreach (var symbol in this.symbols)
            {
                if (symbol.Match(word))
                {
                    return true;
                }
            }
            return false;
        }

        public void Append(string word)
        {
            //the order of this.symbols matters
            foreach (var symbol in this.symbols) {
                
                if (symbol.Match(word)) {
                    if (symbol == Token.Identifier)
                    {
                        bool existed =  this.variables.TryGetValue(word, out var variable);
                        if (!existed)
                        {
                            var item = new Item(word, symbol, ++this.nextAddress);
                            this.List.Add(item);
                            this.variables.Add(word, item);
                        }
                        else
                        {
                            this.List.Add(new Item(word, symbol, variable.Address));
                        }

                    }
                    else
                    {
                        this.List.Add(new Item(word, symbol, 0));
                    }

                    return;
                }
            }
            throw new Exception(word + " not found.");
        }

    }
}
