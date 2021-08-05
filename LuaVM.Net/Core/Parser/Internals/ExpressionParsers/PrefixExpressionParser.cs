using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 前缀表达式解析器
    class PrefixExpressionParser : IBaseExpressionParser
    {
        private ParensExpressionParser parensParser = new ParensExpressionParser();
        private FunctionCallExpressionParser funcParser = new FunctionCallExpressionParser();

        // 解析前缀表达式
        public Expression Parse(Lexer lexer)
        {
            Expression exp = null;
            if (lexer.LookAhead() == TokenType.IDENTIFIER)
            {
                var token = lexer.NextIdentifier();
                exp = new NameExpression(token.line, token.text);
            }
            else
            {
                exp = parensParser.Parse(lexer);
            }
            return FinishPrefixExpression(lexer, exp);
        }

        private Expression FinishPrefixExpression(Lexer lexer, Expression exp)
        {
            while (true)
            {
                switch (lexer.LookAhead())
                {
                    case TokenType.SEP_LABEL:
                        lexer.NextToken();
                        var key1 = ExpressionUtils.ParseExpression(lexer);
                        lexer.NextIdentifier();
                        exp = new TableAccessExpression(lexer.line, exp, key1);
                        break;
                    case TokenType.SEP_DOT:
                        lexer.NextToken();
                        var token = lexer.NextIdentifier();
                        var key2 = new StringExpression(token.line, token.text);
                        exp = new TableAccessExpression(lexer.line, exp, key2);
                        break;
                    case TokenType.SEP_COLON:
                    case TokenType.SEP_LPAREN:
                    case TokenType.SEP_LCURLY:
                    case TokenType.STRING:
                        funcParser.prefix = exp;
                        exp = funcParser.Parse(lexer);
                        break;
                    default:
                        return exp;
                }
            }
        }
    }
}
