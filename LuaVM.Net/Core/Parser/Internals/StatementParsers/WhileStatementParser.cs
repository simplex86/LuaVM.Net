using System;

namespace LuaVM.Net.Core
{
    // while
    class WhileStatementParser : IBaseStatementParser
    {
        private BlockParser blockParser = new BlockParser();

        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_WHILE); // while
            var expression = ExpressionUtils.ParseExpression(lexer);
            lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = blockParser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END); // end

            return new WhileStatement(expression, block);
        }
    }
}
