using System;

namespace LuaVM.Net.Core
{
    // break
    class BreakStatementParser : IBaseStatementParser
    {
        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_BREAK); // break
            return new BreakStatement(lexer.line);
        }
    }
}
