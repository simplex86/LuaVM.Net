using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    // 函数定义表达式
    class FunctionDefineExpressionParser : IBaseExpressionParser
    {
        private BlockParser blockParser = new BlockParser();

        // 解析函数定义表达式
        public Expression Parse(Lexer lexer)
        {
            var line = lexer.line;
            lexer.NextTokenOfType(TokenType.SEP_LPAREN);
            bool isvar = false;
            var args = ParseFunctionArgs(lexer, out isvar); // 参数列表
            lexer.NextTokenOfType(TokenType.SEP_RPAREN);
            var block = blockParser.Parse(lexer);
            var token = lexer.NextTokenOfType(TokenType.KW_END);

            return new FunctionDefineExpression(line, token.line, args, isvar, block);
        }

        // 解析函数的参数列表
        private List<string> ParseFunctionArgs(Lexer lexer, out bool isvar)
        {
            isvar = false;

            switch (lexer.LookAhead())
            {
                case TokenType.SEP_RPAREN:
                    return null;
                case TokenType.VARARG:
                    isvar = true;
                    return null;
            }

            List<string> args = new List<string>();

            var token = lexer.NextIdentifier();
            args.Add(token.text);

            while (lexer.LookAhead() == TokenType.IDENTIFIER)
            {
                lexer.NextToken();
                if (lexer.LookAhead() == TokenType.SEP_COMMA)
                {
                    token = lexer.NextIdentifier();
                    args.Add(token.text);
                }
                else
                {
                    lexer.NextTokenOfType(TokenType.VARARG);
                    isvar = true;
                    break;
                }
            }

            return args;
        }
    }
}
