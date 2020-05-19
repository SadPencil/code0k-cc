using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using code0k_cc.Runtime;
using code0k_cc.Runtime.VariableMap;
using code0k_cc.Standalone;

namespace code0k_cc.Pinocchio
{
    class PinocchioConstraintType : IStandalone
    {
        /*
         *
        Mul,
        Add,
        //MulConst, 
        //MulConstNeg, 
        ZeroP,
        Xor,
        Or,
        Split,
        Pack,
         */

        public (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints)
            Convert(List<(VariableNode inVariableNode, List<PinocchioWire> inPinocchioWires)> inList,
            List<VariableNode> outVariableNodes,
            PinocchioCommonArg commonArg)
        {
            return this.ConvertFunc(inList, outVariableNodes, commonArg);
        }

        private Func<
            List<(VariableNode inVariableNode, List<PinocchioWire> inPinocchioWires)>,
            List<VariableNode>,
            PinocchioCommonArg,
            (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints)
        > ConvertFunc;

        private PinocchioConstraintType() { }

        public static PinocchioConstraintType Mul = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo




            }
        };

        public static PinocchioConstraintType Add = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
            }
        };
        public static PinocchioConstraintType ZeroP = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
            }
        };
        public static PinocchioConstraintType Xor = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
                if (inList.Count == 2)
                {
                    if (inList[0].inVariableNode.Variable.Type == NType.Bool &&
                        inList[1].inVariableNode.Variable.Type == NType.Bool)
                    {
                        //todo
                    }
                    else
                    {
                        throw new Exception("Assert failed.");
                    }
                }
                else
                {
                    throw new Exception("Assert failed.");
                }



            }
        };
        public static PinocchioConstraintType Or = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
            }
        };
        public static PinocchioConstraintType Split = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
            }
        };
        public static PinocchioConstraintType Pack = new PinocchioConstraintType()
        {
            ConvertFunc = (inList, outVariableNodes) =>
            {
                //todo
            }
        };


    }
}
