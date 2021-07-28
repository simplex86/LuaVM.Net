using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 空语句解析器
    class EmptyStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.SEP_SEMI); // ';'
            return new EmptyStatement();
        }
    }

    // break
    class BreakStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_BREAK); // break
            return new BreakStatement(lexer.line);
        }
    }

    // do
    class DoStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_DO);
            Block block = parser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new DoStatement(block);
        }
    }

    // goto
    class GotoStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_GOTO); // goto
            var name = lexer.NextIdentifier().text;

            return new GotoStatement(name);
        }
    }

    // label
    class LabelStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.SEP_LABEL); // '::'
            var name = lexer.NextIdentifier().text;
            lexer.NextTokenOfType(TokenType.SEP_LABEL); // '::'

            return new LabelStatement(name);
        }
    }

    // while
    class WhileStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_WHILE); // while
            Expression expression = null;// parser.ParserExpression(lexer);
            lexer.NextTokenOfType(TokenType.KW_DO);
            var block = parser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new WhileStatement(expression, block);
        }
    }

    // repeat
    class RepeatStatementParser : IBaseParser
    {
        public Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_REPEAT); // repeat
            var block = parser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_UNTIL);
            Expression expression = null;// parser.ParserExpression(lexer);

            return new WhileStatement(expression, block);
        }
    }

    // if

    // for

    // 局部变量和函数定义

    // 赋值和函数调用

    // 非局部函数定义
}
