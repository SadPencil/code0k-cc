using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.Operation;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.Nizk
{
    static class NizkUtils
    {
        public static readonly Variable BoolTrue = new Variable()
        {
            Type = NType.Bool,
            Value = new NizkBoolValue()
            {
                IsConstant = true,
                Value = true,
                VariableType = NizkVariableType.Intermediate,
            }
        };

        public static readonly Variable BoolFalse = new Variable()
        {
            Type = NType.Bool,
            Value = new NizkBoolValue()
            {
                IsConstant = true,
                Value = false,
                VariableType = NizkVariableType.Intermediate,
            }
        };

        public static readonly Variable UInt32Zero = new Variable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = 0,
                VariableType = NizkVariableType.Intermediate,
            }
        };

        public static readonly Variable UInt32One = new Variable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = 1,
                VariableType = NizkVariableType.Intermediate,
            }
        };
        public static readonly Variable UInt32NegOne = new Variable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = System.UInt32.MaxValue, //todo 
                VariableType = NizkVariableType.Intermediate,
            }
        };

        public static Variable NizkCombineFunctionResult(StatementResult result, NType resultNType)
        {
            switch (result)
            {
                case StatementResultOneCase ret:
                    switch (ret.ExecutionResultType)
                    {
                        case StatementResultType.Continue:
                        case StatementResultType.Break:
                            throw new Exception($"Unexpected \"{ ret.ExecutionResultType.ToString()}\" statement.");

                        case StatementResultType.Normal:
                        case StatementResultType.Return:

                            Debug.Assert(( ret.ExecutionResultType != StatementResultType.Normal ) || ret.ReturnVariable == null);
                            if (ret.ReturnVariable != null)
                            {
                                return ret.ReturnVariable.Assign(resultNType);
                            }

                            if (resultNType == NType.Void)
                            {
                                return NType.Void.NewValue();
                            }
                            else
                            {
                                throw new Exception("Missing \"return\" statement.");
                            }

                        default:
                            throw new Exception("Assert failed!");
                    }

                case StatementResultTwoCase ret:
                    var trueVar = NizkCombineFunctionResult(ret.TrueCase, resultNType);
                    var falseVar = NizkCombineFunctionResult(ret.FalseCase, resultNType);
                    return NizkConditionVariable(ret.Condition, trueVar, falseVar);

                default:
                    throw new Exception("Assert failed!");
            }


        }

        public static StatementResult NizkCombineStatementResult(StatementResult result, BasicBlock currentBlock)
        {
            switch (result)
            {
                case StatementResultOneCase ret:
                    return ret;

                case StatementResultTwoCase ret:
                    var trueCase = NizkCombineStatementResult(ret.TrueCase, currentBlock);
                    var falseCase = NizkCombineStatementResult(ret.FalseCase, currentBlock);
                    if (trueCase is StatementResultOneCase trueOneCase && falseCase is StatementResultOneCase falseOneCase)
                    {
                        var trueRetType = trueOneCase.ExecutionResultType;
                        var falseRetType = falseOneCase.ExecutionResultType;
                        if (trueRetType == falseRetType)
                        {
                            switch (trueRetType)
                            {
                                case StatementResultType.Normal:
                                case StatementResultType.Break:
                                case StatementResultType.Continue:
                                    Debug.Assert(trueOneCase.ReturnVariable == null);
                                    Debug.Assert(falseOneCase.ReturnVariable == null);
                                    Overlay retOverlay;
                                    if (trueOneCase.Overlay != falseOneCase.Overlay)
                                    {
                                        Debug.Assert(trueOneCase.Overlay.ParentOverlay == falseOneCase.Overlay.ParentOverlay);
                                        retOverlay = new Overlay(trueOneCase.Overlay.ParentOverlay);

                                        //combine two overlay
                                        NizkCombineOverlay(
                                            ret.Condition,
                                            new OverlayBlock(trueOneCase.Overlay, currentBlock),
                                            new OverlayBlock(falseOneCase.Overlay, currentBlock),
                                            retOverlay
                                        );
                                    }
                                    else
                                    {
                                        retOverlay = trueOneCase.Overlay;
                                    }

                                    return new StatementResultOneCase()
                                    {
                                        ExecutionResultType = trueRetType,
                                        Overlay = retOverlay,
                                    };

                                case StatementResultType.Return:
                                    // currently, not combining "return"
                                    return ret;

                                default:
                                    throw new Exception("Assert failed!");
                            }

                        }
                        else
                        {
                            return ret;
                        }
                    }
                    else
                    {
                        return new StatementResultTwoCase()
                        {
                            Condition = ret.Condition,
                            FalseCase = falseCase,
                            TrueCase = trueCase,
                        };
                    }

                default:
                    throw new Exception("Assert failed!");
            }

        }

        public static void NizkCombineOverlay(Variable nizkConditionVariable, OverlayBlock trueOverlayBlock, OverlayBlock falseOverlayBlock, Overlay retOverlay)
        {
            Debug.Assert(nizkConditionVariable.Type == NType.Bool);
            Debug.Assert(trueOverlayBlock.Block == falseOverlayBlock.Block);
            Debug.Assert(trueOverlayBlock.Overlay.ParentOverlay == retOverlay.ParentOverlay && falseOverlayBlock.Overlay.ParentOverlay == retOverlay.ParentOverlay);


            Overlay trueOverlay = trueOverlayBlock.Overlay;
            Overlay falseOverlay = falseOverlayBlock.Overlay;

            for (var block = trueOverlayBlock.Block; block != null; block = block.ParentBlock)
            {
                var retOverlayBlock = new OverlayBlock(retOverlay, block);
                var nameList = block.Variables[trueOverlay].Keys.Union(block.Variables[falseOverlay].Keys).ToList();

                foreach (var name in nameList)
                {
                    Variable retVar = null;

                    Variable trueVar = block.Variables[trueOverlay].GetValueOrDefault(name).Variable;
                    Variable falseVar = block.Variables[falseOverlay].GetValueOrDefault(name).Variable;

                    Variable parentVar = null;
                    for (var ol = trueOverlay.ParentOverlay; ol != null; ol = ol.ParentOverlay)
                    {
                        parentVar = block.Variables[trueOverlay.ParentOverlay]?.GetValueOrDefault(name).Variable;
                        if (parentVar != null) break;
                    }
                    Debug.Assert(( trueVar != null ) || ( falseVar != null ));

                    // note that parentVar is nullable
                    if (trueVar == null)
                    {
                        trueVar = parentVar;
                    }

                    if (falseVar == null)
                    {
                        falseVar = parentVar;
                    }

                    if (( trueVar == falseVar ))
                    {
                        Debug.Assert(trueVar != null);
                        retVar = trueVar;
                    }
                    else if (trueVar == null)
                    {
                        retVar = falseVar;
                    }
                    else if (falseVar == null)
                    {
                        retVar = trueVar;
                    }
                    else
                    {
                        // generate new value
                        retVar = NizkConditionVariable(nizkConditionVariable, trueVar, falseVar);
                    }


                    //add var to retOverlay
                    Debug.Assert(retVar != null);
                    retOverlayBlock.AddVariable(name, retVar, true);
                }


            }

        }

        public static Variable NizkConditionVariable(Variable condition, Variable trueVar, Variable falseVar)
        {
            if (trueVar.Type == NType.Bool)
            {
                var var1 = condition;
                var var2 = NType.UInt32.ExplicitConvert(trueVar, NType.UInt32);
                var var3 = NType.UInt32.ExplicitConvert(falseVar, NType.UInt32); ;

                var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne, BinaryOperation.Multiplication);
                var var5 = NType.UInt32.BinaryOperation(var2, var4, BinaryOperation.Addition);
                var var6 = NType.UInt32.BinaryOperation(var5, var1, BinaryOperation.Multiplication);
                var var7 = NType.UInt32.BinaryOperation(var3, var6, BinaryOperation.Addition);

                var var8 = NType.UInt32.ExplicitConvert(var7, NType.Bool);
                return var8;
            }
            else if (trueVar.Type == NType.UInt32)
            {
                var var1 = condition;
                var var2 = trueVar;
                var var3 = falseVar;

                var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne, BinaryOperation.Multiplication);
                var var5 = NType.UInt32.BinaryOperation(var2, var4, BinaryOperation.Addition);
                var var6 = NType.UInt32.BinaryOperation(var5, var1, BinaryOperation.Multiplication);
                var var7 = NType.UInt32.BinaryOperation(var3, var6, BinaryOperation.Addition);

                // maybe needs a mod 2^32?
                // currently, maybe not because it is handled at other operations
                return var7;
            }
            else
            {
                throw new Exception($"Unsupported type \"{trueVar.Type.TypeCodeName}\" in a nizk-condition structure.");
            }

        }

    }
}
