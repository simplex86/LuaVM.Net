using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // if
    class IfStatementParser : IBaseStatementParser
    {
        private BlockParser blockParser = new BlockParser();

        public Statement Parse(Lexer lexer)
        {
            List<Expression> expressions = new List<Expression>(4);
            List<Block> blocks = new List<Block>(4);

            lexer.NextTokenOfType(TokenType.KW_IF); // if
            var exp = ExpressionUtils.ParseExpression(lexer);
            expressions.Add(exp);
            lexer.NextTokenOfType(TokenType.KW_THEN); // then
            var block = blockParser.Parse(lexer);
            blocks.Add(block);

            while (lexer.LookAhead() == TokenType.KW_ELSEIF)
            {
                lexer.NextToken();

                exp = ExpressionUtils.ParseExpression(lexer);
                expressions.Add(exp);
                lexer.NextTokenOfType(TokenType.KW_THEN); // then
                block = blockParser.Parse(lexer);
                blocks.Add(block);
            }

            if (lexer.LookAhead() == TokenType.KW_ELSE)
            {
                lexer.NextToken();

                exp = ExpressionUtils.ParseExpression(lexer);
                expressions.Add(exp);
                block = blockParser.Parse(lexer);
                blocks.Add(block);
            }

            lexer.NextTokenOfType(TokenType.KW_END); // end
            return new IfStatement(expressions, blocks);
        }
    }
}
