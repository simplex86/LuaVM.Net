using System;

namespace LuaVM.Net.Core
{
    // label
    class LabelStatementParser : IBaseStatementParser
    {
        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.SEP_LABEL); // '::'
            var name = lexer.NextIdentifier().text;
            lexer.NextTokenOfType(TokenType.SEP_LABEL); // '::'

            return new LabelStatement(name);
        }
    }
}
