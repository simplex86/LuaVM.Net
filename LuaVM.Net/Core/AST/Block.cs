using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 块，即Lua可执行的最小单位
    public class Block
    {
        // 语句列表
        public List<Statement>  statements  { get; private set; } = new List<Statement>();
        // 表达式列表
        public List<Expression> expressions { get; private set; } = new List<Expression>();
        // 最后一行
        public int lastline { get; set; } = 0;
    }
}
