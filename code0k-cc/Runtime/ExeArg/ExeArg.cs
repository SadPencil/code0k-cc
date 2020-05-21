using System;
using System.Collections.Generic;
using System.IO;
using code0k_cc.Parse;
using code0k_cc.Runtime.Block;

namespace code0k_cc.Runtime.ExeArg
{
    class ExeArg : ICloneable
    {
        public OverlayBlock Block;

        /// <summary>
        /// To count on how many times the function has been called.
        /// </summary>
        public CallStack CallStack;

        /// <summary>
        /// Debug purpose only. Do not use under an nizk-if/while statements.
        /// </summary>
        public TextWriter StdOut; 

        public ExeArg(OverlayBlock block, CallStack callStack, TextWriter stdOut)
        {
            this.Block = block;
            this.CallStack = callStack; 
            this.StdOut = stdOut;
        }

        /// <summary>
        /// Memberwise Clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
