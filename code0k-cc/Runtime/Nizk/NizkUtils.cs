using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using code0k_cc.Runtime.Block;
using code0k_cc.Runtime.ExeResult;
using code0k_cc.Runtime.ValueOfType;

namespace code0k_cc.Runtime.Nizk
{
    static class NizkUtils
    {
        public static readonly Variable BoolTrue = new Variable(new RawVariable()
        {
            Type = NType.Bool,
            Value = new NizkBoolValue()
            {
                IsConstant = true,
                Value = true,
            }
        });

        public static readonly Variable BoolFalse = new Variable(new RawVariable()
        {
            Type = NType.Bool,
            Value = new NizkBoolValue()
            {
                IsConstant = true,
                Value = false,
            }
        });

        public static readonly Variable UInt32Zero = new Variable(new RawVariable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = 0,
            }
        });

        public static readonly Variable UInt32One = new Variable(new RawVariable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = 1,
            }
        });

        public static readonly Variable UInt32NegOne = new Variable(new RawVariable()
        {
            Type = NType.UInt32,
            Value = new NizkUInt32Value()
            {
                IsConstant = true,
                Value = System.UInt32.MaxValue, //todo  
            }
        });

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
                                return NType.Void.GetNewEmptyVariable();
                            }
                            else
                            {
                                throw new Exception("Missing \"return\" statement.");
                            }

                        default:
                            throw CommonException.AssertFailedException();
                    }

                case StatementResultTwoCase ret:
                    var trueVar = NizkCombineFunctionResult(ret.TrueCase, resultNType);
                    var falseVar = NizkCombineFunctionResult(ret.FalseCase, resultNType);
                    return NizkConditionVariable(ret.Condition, trueVar, falseVar);

                default:
                    throw CommonException.AssertFailedException();
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
                                        retOverlay = trueOneCase.Overlay.ParentOverlay;

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
                                        //todo is it possible or not?
                                        Debug.Assert(false);
                                        throw new NotImplementedException();
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
                                    throw CommonException.AssertFailedException();
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
                    throw CommonException.AssertFailedException();
            }

        }

        public static void NizkCombineOverlay(Variable nizkConditionVariable, OverlayBlock trueOverlayBlock, OverlayBlock falseOverlayBlock, Overlay retOverlay)
        {
            Debug.Assert(nizkConditionVariable.Type == NType.Bool);
            Debug.Assert(trueOverlayBlock.Block == falseOverlayBlock.Block);
            Debug.Assert(trueOverlayBlock.Overlay.ParentOverlay == falseOverlayBlock.Overlay.ParentOverlay);
            //Debug.Assert(trueOverlayBlock.Overlay.ParentOverlay == retOverlay);

            Overlay trueOverlay = trueOverlayBlock.Overlay;
            Overlay falseOverlay = falseOverlayBlock.Overlay;
            Overlay parentOverlay = trueOverlay.ParentOverlay;

            for (var block = trueOverlayBlock.Block; block != null; block = block.ParentBlock)
            {
                var newRetOverlayBlock = new OverlayBlock(retOverlay, block);
                var newTrueOverlayBlock = new OverlayBlock(trueOverlay, block);
                var newFalseOverlayBlock = new OverlayBlock(falseOverlay, block);
                var newParentOverlayBlock = new OverlayBlock(parentOverlay, block);

                var nameList = newTrueOverlayBlock.GetVariableDict().Keys.Union(newFalseOverlayBlock.GetVariableDict().Keys).ToList();

                foreach (var name in nameList)
                {
                    Variable retVar = null;

                    Variable trueVar = newTrueOverlayBlock.GetVariableRefRef(name, false, false)?.VariableRef?.Variable;
                    Variable falseVar = newFalseOverlayBlock.GetVariableRefRef(name, false, false)?.VariableRef?.Variable;
                    Debug.Assert(( trueVar != null ) || ( falseVar != null ));

                    Variable parentVar = null;
                    for (var ol = trueOverlay.ParentOverlay; ol != null; ol = ol.ParentOverlay)
                    {
                        parentVar = new OverlayBlock(ol, block).GetVariableRefRef(name, false, false)?.VariableRef?.Variable;
                        if (parentVar != null) break;
                    }

                    // note that parentVar is nullable
                    if (parentVar != null)
                    {
                        if (trueVar == null)
                        {
                            trueVar = parentVar;
                        }

                        if (falseVar == null)
                        {
                            falseVar = parentVar;
                        }
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
                    newRetOverlayBlock.AddVariable(name, retVar, true);
                }


            }

        }

        public static Variable NizkConditionVariable(Variable condition, Variable trueVar, Variable falseVar)
        {
            if (trueVar.Type == NType.Bool)
            {
                var var1 = condition.ExplicitConvert(NType.UInt32);
                var var2 = trueVar.ExplicitConvert(NType.UInt32);
                var var3 = falseVar.ExplicitConvert(NType.UInt32);

                var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne, VariableOperationType.Binary_Multiplication);
                var var5 = NType.UInt32.BinaryOperation(var2, var4, VariableOperationType.Binary_Addition);
                var var6 = NType.UInt32.BinaryOperation(var5, var1, VariableOperationType.Binary_Multiplication);
                var var7 = NType.UInt32.BinaryOperation(var3, var6, VariableOperationType.Binary_Addition);

                var var8 = var7.ExplicitConvert(NType.Bool);
                return var8;
            }
            else if (trueVar.Type == NType.UInt32)
            {
                var var1 = condition.ExplicitConvert(NType.UInt32);
                var var2 = trueVar;
                var var3 = falseVar;

                var var4 = NType.UInt32.BinaryOperation(var3, UInt32NegOne, VariableOperationType.Binary_Multiplication);
                var var5 = NType.UInt32.BinaryOperation(var2, var4, VariableOperationType.Binary_Addition);
                var var6 = NType.UInt32.BinaryOperation(var5, var1, VariableOperationType.Binary_Multiplication);
                var var7 = NType.UInt32.BinaryOperation(var3, var6, VariableOperationType.Binary_Addition);

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
