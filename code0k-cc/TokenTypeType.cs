namespace code0k_cc
{ 
        public enum TokenTypeType
        {
            /// <summary>
            /// 关键字，需要空格分隔
            /// </summary>
            Keyword,
            /// <summary>
            /// 自定义标识符
            /// </summary>
            Identifier,
            /// <summary>
            /// 整数
            /// </summary>
            Number,
            /// <summary>
            /// 与 Keyword 不同的是，不需要加空格
            /// </summary>
            NonLetterKeyword
        } 

}
