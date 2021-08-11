using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 语法分析器
    // https://www.lua.org/manual/5.3/manual.html#9
    public class Parser
    {
        // 语法分析
        public Block Parse(Lexer lexer)
        {
            BlockParser parser = new BlockParser();
            var block = parser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.EOF);

            return block;
        }
    }
}
