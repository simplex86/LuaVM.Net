using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 块，即Lua可执行的最小单位
    public class Block
    {
        // 语句列表
        public List<Statement> statements { get; private set; } = null;
        // 表达式列表
        public List<Expression> expressions { get; private set; } = null;
        // 最后一行
        public int lastline { get; private set; } = 0;

        public Block(List<Statement> statements, List<Expression> expressions, int lastline)
        {
            this.statements = statements;
            this.expressions = expressions;
            this.lastline = lastline;
        }
    }
}
