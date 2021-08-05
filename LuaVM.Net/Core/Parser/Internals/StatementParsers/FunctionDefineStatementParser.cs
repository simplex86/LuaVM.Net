using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 非局部函数定义
    class FunctionDefineStatementParser : IBaseStatementParser
    {
        FunctionDefineExpressionParser funcParser = new FunctionDefineExpressionParser();

        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_FUNCTION);

            bool hasColon = false;
            var nameExp = ParseFunctionName(lexer, out hasColon);
            var funcExp = funcParser.Parse(lexer) as FunctionDefineExpression;

            if (hasColon)
            {
                funcExp.args.Add("");
                funcExp.args[0] = "self";
            }

            return new AssignStatement(funcExp.line, new List<Expression> { nameExp }, new List<Expression> { funcExp });
        }

        // 解析函数名字
        private Expression ParseFunctionName(Lexer lexer, out bool hasColon)
        {
            hasColon = false;

            var token = lexer.NextIdentifier();
            Expression exp = new NameExpression(token.line, token.text);

            while(lexer.LookAhead() == TokenType.SEP_DOT)
            {
                lexer.NextToken();
                token = lexer.NextIdentifier();
                var idx = new StringExpression(token.line, token.text);
                exp = new TableAccessExpression(token.line, exp, idx);
            }

            if (lexer.LookAhead() == TokenType.SEP_COLON)
            {
                hasColon = true;

                lexer.NextToken();
                token = lexer.NextIdentifier();
                var idx = new StringExpression(token.line, token.text);
                exp = new TableAccessExpression(token.line, exp, idx);
            }

            return exp;
        }
    }
}
