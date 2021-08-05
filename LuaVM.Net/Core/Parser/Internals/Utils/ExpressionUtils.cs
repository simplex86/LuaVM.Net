using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    static class ExpressionUtils
    {
        private static OperatorExpressionParser operParser = new OperatorExpressionParser();

        // 解析表达式列表
        public static List<Expression> ParseExpressions(Lexer lexer)
        {
            List<Expression> expressions = new List<Expression>();

            var exp = ParseExpression(lexer);
            expressions.Add(exp);

            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                exp = ParseExpression(lexer);
                expressions.Add(exp);
            }

            return expressions;
        }

        // 解析（单个）表达式
        public static Expression ParseExpression(Lexer lexer)
        {
            return operParser.Parse(lexer);
        }
    }
}
