using System;
using System.Collections.Generic;
using System.Text;

namespace code0k_cc.CustomException
{
    class CommonException
    {
        public static Exception AssertFailedException() => new Exception("Assert failed!");
    }
}
