using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    class FunctionCallExpressionParser : IBaseExpressionParser
    {
        public Expression prefix { get; set; } = null;

        public Expression Parse(Lexer lexer)
        {
            var name = ParseNameExpression(lexer);
            var line1 = lexer.line;
            var args = ParseFunctionCallArgsExpressions(lexer);
            var line2 = lexer.line;

            return new FunctionCallExpression(line1, line2, prefix, name, args);
        }

        private StringExpression ParseNameExpression(Lexer lexer)
        {
            if (lexer.LookAhead() == TokenType.SEP_COLON)
            {
                lexer.NextToken();
                var token = lexer.NextIdentifier();
                return new StringExpression(token.line, token.text);
            }

            return null;
        }

        private List<Expression> ParseFunctionCallArgsExpressions(Lexer lexer)
        {
            List<Expression> args = null;

            switch (lexer.LookAhead())
            {
                case TokenType.SEP_LPAREN:
                    lexer.NextToken();
                    if (lexer.LookAhead() == TokenType.SEP_RPAREN)
                    {
                        args = ExpressionUtils.ParseExpressions(lexer);
                    }
                    lexer.NextTokenOfType(TokenType.SEP_RPAREN);
                    break;
                case TokenType.SEP_LCURLY:
                    break;
                default:
                    break;
            }

            return args;
        }
    }
}
