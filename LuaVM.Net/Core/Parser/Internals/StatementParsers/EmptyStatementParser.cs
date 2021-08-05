using System;

namespace LuaVM.Net.Core
{
    // 空语句解析器
    class EmptyStatementParser : IBaseStatementParser
    {
        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.SEP_SEMI); // ';'
            return new EmptyStatement();
        }
    }
}
