using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 表构造表达式解析器
    class TableConstructorExpressionParser : IBaseExpressionParser
    {
        public Expression Parse(Lexer lexer)
        {
            var line = lexer.line;
            lexer.NextTokenOfType(TokenType.SEP_LCURLY);
            List<Expression> ks = new List<Expression>();
            List<Expression> vs = new List<Expression>();
            ParseFields(lexer, ks, vs);
            lexer.NextTokenOfType(TokenType.SEP_RCURLY);
            var lastline = lexer.line;

            return new TableConstructionExpression(line, lastline, ks, vs);
        }

        private void ParseFields(Lexer lexer, List<Expression> ks, List<Expression> vs)
        {
            if (lexer.LookAhead() != TokenType.SEP_RCURLY)
            {
                var kv = ParseField(lexer);
                ks.Add(kv[0]);
                vs.Add(kv[1]);

                while (IsFieldSeparator(lexer.LookAhead()))
                {
                    lexer.NextToken();
                    if (lexer.LookAhead() == TokenType.SEP_RCURLY)
                    {
                        break;
                    }

                    kv = ParseField(lexer);
                    ks.Add(kv[0]);
                    vs.Add(kv[1]);
                }
            }
        }

        private Expression[] ParseField(Lexer lexer)
        {
            Expression[] kv = new Expression[2] { null, null };

            if (lexer.LookAhead() == TokenType.SEP_LBRACK)
            {
                lexer.NextToken();
                kv[0] = ExpressionUtils.ParseExpression(lexer);
                lexer.NextTokenOfType(TokenType.SEP_RBRACK);
                lexer.NextTokenOfType(TokenType.OP_ASSIGN);
                kv[1] = ExpressionUtils.ParseExpression(lexer);
            }
            else
            {
                var exp = ExpressionUtils.ParseExpression(lexer);
                kv[1] = exp;

                NameExpression nameExp = exp as NameExpression;
                if (nameExp != null)
                {
                    if (lexer.LookAhead() == TokenType.OP_ASSIGN)
                    {
                        lexer.NextToken();
                        kv[0] = new StringExpression(nameExp.line, nameExp.value);
                        kv[1] = ExpressionUtils.ParseExpression(lexer);
                    }
                }
            }

            return kv;
        }

        // 是否是分隔符
        private bool IsFieldSeparator(int type)
        {
            return type == TokenType.SEP_COMMA || type == TokenType.SEP_SEMI;
        }
    }
}
