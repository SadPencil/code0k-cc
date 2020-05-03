using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace code0k_cc.Runtime.Block
{
    class OverlayBlock
    {
        public Overlay Overlay { get; }
        public BasicBlock Block { get; }

        public OverlayBlock(Overlay overlay, BasicBlock block)
        {
            this.Overlay = overlay;
            this.Block = block;
        }

        public override string ToString()
        {
            return $"OverlayBlock{{Block:{this.Block.ToString()},Overlay:{this.Overlay.ToString()}}}";
        }

        public OverlayBlock LocateVariableBlock(string name, bool throwException)
        {
            // block first, overlay second
            for (var block = this.Block; block != null; block = block.ParentBlock)
            {
                for (var overlay = this.Overlay; overlay != null; overlay = overlay.ParentOverlay)
                { 
                    if (block.GetVariableDict(overlay).ContainsKey(name))
                    {
                        return new OverlayBlock(overlay, block);
                    }

                }
            }

            if (throwException)
            {
                throw new Exception($"Unexpected variable \"{name}\".");
            }
            else
            {
                return null;
            }

        }

        public VariableRefRef GetVariableRefRef(string name, bool recursively, bool throwException)
        {
            VariableRef varRef = null;
            Overlay overlay;
            BasicBlock block;

            if (recursively)
            {
                var overlayBlock = this.LocateVariableBlock(name, throwException);
                if (overlayBlock == null)
                {
                    Debug.Assert(!throwException);
                    return null;
                }

                overlay = overlayBlock.Overlay;
                block = overlayBlock.Block;
                varRef = block.GetVariableDict(overlay)[name];

            }
            else
            {
                block = this.Block;
                for (overlay = this.Overlay; overlay != null; overlay = overlay.ParentOverlay)
                {
                    if (block.GetVariableDict(overlay).ContainsKey(name))
                    {
                        varRef = block.GetVariableDict(overlay)[name];
                        break;
                    }
                }

                if (varRef == null)
                {
                    if (throwException)
                    {
                        throw new Exception($"Unexpected variable \"{name}\".");
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            // important: copy the variableRef to this overlay
            if (overlay != this.Overlay)
            {
                Debug.Assert(!( block.GetVariableDict(this.Overlay).ContainsKey(name) ));

                // make a copy of this variableRef
                var newRef = new VariableRef() { Variable = varRef.Variable };
                block.GetVariableDict(this.Overlay).Add(name, newRef);
                overlay = this.Overlay;
                //block = block;
                varRef = newRef;
            }

            return new VariableRefRef(varRef);
        }


        public void AddVariable(string name, Variable value, bool updateIfExist)
        {
            // GetVariableRefRef() ensures that `variableRefRef` must be contained in this overlay
            var variableRefRef = this.GetVariableRefRef(name, false, false);

            if (variableRefRef != null)
            {
                if (updateIfExist)
                {

                    // check the type
                    if (value.Type != variableRefRef.VariableRef.Variable.Type)
                    {
                        throw new Exception($"Type mismatch while assigning variable \"{name}\". " +
                                            $"Expected \"{ variableRefRef.VariableRef.Variable.Type.TypeCodeName}\", got \"{value.Type.TypeCodeName}\".");
                    }

                    // only replace the variable, not the reference
                    variableRefRef.VariableRef.Variable = value;

                }
                else
                {
                    throw new Exception($"Variable \"{name}\" has already been declared at this scope.");
                }
            }
            else
            {
                this.Block.GetVariableDict(this.Overlay).Add(name, new VariableRef() { Variable = value });
            }
        }


    }
}
