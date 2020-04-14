using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code0k_cc
{
    [System.Diagnostics.DebuggerNonUserCode]
    class ParseException : Exception
    {
        public ParseException() : base() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message,Exception innerException) : base(message, innerException) { }
    }
}
