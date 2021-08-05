using System;

namespace LuaVM.Net.Core
{
    // repeat
    class RepeatStatementParser : IBaseStatementParser
    {
        private BlockParser blockParser = new BlockParser();

        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_REPEAT); // repeat
            var block = blockParser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_UNTIL); // until
            var expression = ExpressionUtils.ParseExpression(lexer);

            return new WhileStatement(expression, block);
        }
    }
}
