using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 普通表达式解析器
    class CommonExpressionParser : IBaseExpressionParser
    {
        private PrefixExpressionParser prefixParser = new PrefixExpressionParser();
        private NumberExpressionParser numberParser = new NumberExpressionParser();
        private TableConstructorExpressionParser tableParser = new TableConstructorExpressionParser();
        private FunctionDefineExpressionParser functionParser = new FunctionDefineExpressionParser();

        public Expression Parse(Lexer lexer)
        {
            switch (lexer.LookAhead())
            {
                case TokenType.VARARG:
                    var a = lexer.NextToken();
                    return new VarargExpression(a.line);
                case TokenType.KW_NIL:
                    var b = lexer.NextToken();
                    return new NilExpression(b.line);
                case TokenType.KW_TRUE:
                    var c = lexer.NextToken();
                    return new TrueExpression(c.line);
                case TokenType.KW_FALSE:
                    var d= lexer.NextToken();
                    return new FalseExpression(d.line);
                case TokenType.STRING:
                    var e = lexer.NextToken();
                    return new StringExpression(e.line, e.text);
                case TokenType.NUMBER:
                    return numberParser.Parse(lexer);
                case TokenType.SEP_LCURLY:
                    return tableParser.Parse(lexer);
                case TokenType.KW_FUNCTION:
                    lexer.NextToken();
                    return functionParser.Parse(lexer);
                default:
                    // prefix expression
                    break;
            }

            return prefixParser.Parse(lexer);
        }
    }
}
