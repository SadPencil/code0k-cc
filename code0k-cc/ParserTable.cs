using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code0k_cc
{
    abstract class ParserTable
    { 
        public class Item
        {
            public string Type;
            public List<Item> Items = new List<Item>();
            public string Value;

            public override string ToString()
            {
                string output = this.Type;

                if (!String.IsNullOrEmpty(this.Value))
                {
                    output += ' ' + this.Value;
                }

                if (this.Items.Count != 0)
                {
                    output += " [" + this.Items.Count + ']';
                }
                return output;
            }
        }
    }
}
