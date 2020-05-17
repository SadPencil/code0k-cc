using System;
using System.Collections.Generic;
using System.Text;
using code0k_cc.Standalone;

namespace code0k_cc.Runtime.Block
{
    class Overlay : IStandalone
    {
        public Overlay ParentOverlay { get; }

        public Overlay(Overlay parent)
        {
            this.ParentOverlay = parent;
        }

        public override string ToString()
        {
            return "Overlay " + this.Guid;
        }
    }
}
