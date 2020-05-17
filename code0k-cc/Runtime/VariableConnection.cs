using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Immutable;

namespace code0k_cc.Runtime
{
    class VariableConnection : IImmutable
    {
        public VariableConnectionType Type;
        public List<Variable> InVariables { get; } = new List<Variable>();
        public List<Variable> OutVariables { get; } = new List<Variable>();

    }
}
