using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
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

        public (List<PinocchioWire> Wires, List<PinocchioConstraint> Constraints) Convert(
            List<(VariableNode inVariableNode, List<PinocchioWire> inPinocchioWires)> inList,
            List<VariableNode> outVariableNodes)
        {
            return  this.ConvertFunc(inList, outVariableNodes);
        }

        private Func<
            List<(VariableNode inVariableNode, List<PinocchioWire> inPinocchioWires)>,
            List<VariableNode>,
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
