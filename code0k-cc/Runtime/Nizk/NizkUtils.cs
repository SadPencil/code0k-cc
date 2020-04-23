using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.Operation;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.Nizk
{
    static class NizkUtils
    {
        public static readonly VariableRef UInt32One = new VariableRef()
        {
            Variable = new Variable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = true,
                    Value = 1,
                    VariableType = NizkVariableType.Intermediate,
                }
            }
        };

        public static readonly VariableRef UInt32NegOne = new VariableRef()
        {
            Variable = new Variable()
            {
                Type = NType.UInt32,
                Value = new NizkUInt32Value()
                {
                    IsConstant = true,
                    Value = -1,
                    VariableType = NizkVariableType.Intermediate,
                }
            }
        };

        public static void NizkCombineOverlay(Variable nizkConditionVariable, OverlayBlock trueOverlayBlock, OverlayBlock falseOverlayBlock, Overlay retOverlay)
        {
            Debug.Assert(nizkConditionVariable.Type == NType.Bool);
            Debug.Assert(trueOverlayBlock.Block == falseOverlayBlock.Block);


            Overlay trueOverlay = trueOverlayBlock.Overlay;
            Overlay falseOverlay = falseOverlayBlock.Overlay;

            for (var block = trueOverlayBlock.Block; block != null; block = block.ParentBlock)
            {
                var nameList = block.Variables[trueOverlay].Keys.Union(block.Variables[falseOverlay].Keys).ToList();

                foreach (var name in nameList)
                {
                    var trueVar = block.Variables[trueOverlay].GetValueOrDefault(name);
                    var falseVar = block.Variables[falseOverlay].GetValueOrDefault(name);
                    var parentVar = block.Variables[trueOverlay.ParentOverlay]?.GetValueOrDefault(name);

                    Debug.Assert(( trueVar != null ) || ( falseVar != null ));
                    if (trueVar == null)
                    {
                        trueVar = new VariableRef() { Variable = parentVar?.Variable };
                    }

                    if (falseVar == null)
                    {
                        falseVar = new VariableRef() { Variable = parentVar?.Variable };
                    }

                    if (( parentVar != null ) && ( ( trueVar == null || trueVar?.Variable == parentVar.Variable ) && ( falseVar == null || falseVar?.Variable == parentVar.Variable ) ))
                    {
                        // well, not actually changed
                        continue;
                    }

                    // generate new value
                    if (trueVar.Variable.Type != falseVar.Variable.Type)
                    {
                        throw new Exception($"Type mismatched. Got \"{trueVar.Variable.Type.TypeCodeName}\" and \"{falseVar.Variable.Type.TypeCodeName}\"!");
                    }

                    if (trueVar.Variable.Type == NType.Bool)
                    {
                        var var1 = nizkConditionVariable;
                        var var2 = trueVar.Variable;
                        var var3 = falseVar.Variable;

                        var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne.Variable, BinaryOperation.Multiplication);
                        var var5 = NType.UInt32.BinaryOperation(var2, var4, BinaryOperation.Addition);
                        var var6 = NType.UInt32.BinaryOperation(var5, var1, BinaryOperation.Multiplication);
                        var var7 = NType.UInt32.BinaryOperation(var3, var6, BinaryOperation.Addition);

                        //todo var8: var8:=Bool(var7)
                    }
                    else if (trueVar.Variable.Type == NType.UInt32)
                    {
                        var var1 = nizkConditionVariable;
                        var var2 = trueVar.Variable;
                        var var3 = falseVar.Variable;

                        var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne.Variable, BinaryOperation.Multiplication);
                        var var5 = NType.UInt32.BinaryOperation(var2, var4, BinaryOperation.Addition);
                        var var6 = NType.UInt32.BinaryOperation(var5, var1, BinaryOperation.Multiplication);
                        var var7 = NType.UInt32.BinaryOperation(var3, var6, BinaryOperation.Addition);

                        //todo maybe needs a mod 2^32?
                    }
                    else
                    {
                        throw new Exception($"Unsupported type \"{trueVar.Variable.Type}\" in a nizk-condition structure.");
                    }
                }


            }

        }

    }
}
