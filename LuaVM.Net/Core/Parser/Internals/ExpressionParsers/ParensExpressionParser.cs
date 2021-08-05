using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    class ParensExpressionParser : IBaseExpressionParser
    {
        public Expression Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.SEP_LPAREN);
            var exp = ExpressionUtils.ParseExpression(lexer);
            lexer.NextTokenOfType(TokenType.SEP_RPAREN);

            var type = exp.GetType();
            if (type == typeof(VarargExpression) ||
                type == typeof(FunctionCallExpression) ||
                type == typeof(NameExpression) ||
                type == typeof(TableAccessExpression))
            {
                return new ParensExpression(exp);
            }

            return exp;
        }
    }
}
