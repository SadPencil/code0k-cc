using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc
{
    class NizkNode
    { 
        public readonly List<NizkNode> InNodes = new List<NizkNode>();
        public readonly List<NizkNode> OutNodes = new List<NizkNode>(); 
    }
}
