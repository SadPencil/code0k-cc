using System.Collections.Generic;

namespace code0k_cc.Nizk
{
    class NizkNode
    { 
        public readonly List<NizkNode> InNodes = new List<NizkNode>();
        public readonly List<NizkNode> OutNodes = new List<NizkNode>(); 
    }
}
