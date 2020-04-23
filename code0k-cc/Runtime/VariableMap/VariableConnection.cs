using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.Runtime.VariableMap
{
    class VariableConnection
    {
        public VariableConnectionType Type;
        public List<Variable> InVariables { get; } = new List<Variable>();
        public List<Variable> OutVariables { get; } = new List<Variable>();
        
    }
}
