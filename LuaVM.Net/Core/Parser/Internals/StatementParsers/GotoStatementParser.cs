using System;

namespace LuaVM.Net.Core
{
    // goto
    class GotoStatementParser : IBaseStatementParser
    {
        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_GOTO); // goto
            var name = lexer.NextIdentifier().text;

            return new GotoStatement(name);
        }
    }
}
