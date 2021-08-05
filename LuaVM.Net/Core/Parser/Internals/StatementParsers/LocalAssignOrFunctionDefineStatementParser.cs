using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 局部变量和函数定义
    class LocalAssignOrFunctionDefineStatementParser : IBaseStatementParser
    {
        FunctionDefineExpressionParser funcParser = new FunctionDefineExpressionParser();

        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_LOCAL); // local
            if (lexer.LookAhead() == TokenType.KW_FUNCTION)
            {
                return FinishLocalFunctionDefineStatement(lexer);
            }

            return FinishLocalVariateDefineStatement(lexer);
        }

        // 局部函数定义
        private Statement FinishLocalFunctionDefineStatement(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_FUNCTION);
            var token = lexer.NextIdentifier();
            var expression = funcParser.Parse(lexer);

            return new LocalFunctionDefineStatement(token.text, expression);
        }

        // 局部变量定义
        private Statement FinishLocalVariateDefineStatement(Lexer lexer)
        {
            var token = lexer.NextIdentifier();
            var names = VariateUtils.GetNames(lexer, token.text);

            List<Expression> exps = null;
            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                lexer.NextToken();
                exps = ExpressionUtils.ParseExpressions(lexer);
            }

            return new LocalVariateStatement(lexer.line, names, exps);
        }
    }
}
