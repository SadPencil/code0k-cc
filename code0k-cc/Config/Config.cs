using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace code0k_cc.Config
{
    class Config
    {
        public BigInteger ModulusPrimeField_Prime;
        public int ModulusPrimeField_Prime_Bit;

        public Config()
        {
            this.ModulusPrimeField_Prime = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");
            this.ModulusPrimeField_Prime_Bit = 254; // ensure that: 2^ModulusPrimeField_Prime_Bit <= ModulusPrimeField_Prime < 2^(ModulusPrimeField_Prime_Bit+1)

        }
    }
}
