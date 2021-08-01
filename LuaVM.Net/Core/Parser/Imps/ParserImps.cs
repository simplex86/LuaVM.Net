using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 空语句解析器
    class EmptyStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.SEP_SEMI); // ';'
            return new EmptyStatement();
        }
    }

    // break
    class BreakStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_BREAK); // break
            return new BreakStatement(lexer.line);
        }
    }

    // do
    class DoStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_DO);
            Block block = parser.ParseBlock(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new DoStatement(block);
        }
    }

    // goto
    class GotoStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_GOTO); // goto
            var name = lexer.NextIdentifier().text;

            return new GotoStatement(name);
        }
    }

    // label
    class LabelStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
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
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_WHILE); // while
            var expression = parser.ParseExpression(lexer);
            lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = parser.ParseBlock(lexer);
            lexer.NextTokenOfType(TokenType.KW_END); // end

            return new WhileStatement(expression, block);
        }
    }

    // repeat
    class RepeatStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_REPEAT); // repeat
            var block = parser.ParseBlock(lexer);
            lexer.NextTokenOfType(TokenType.KW_UNTIL); // until
            var expression = parser.ParseExpression(lexer);

            return new WhileStatement(expression, block);
        }
    }

    // if
    class IfStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            List<Expression> expressions = new List<Expression>(4);
            List<Block> blocks = new List<Block>(4);

            lexer.NextTokenOfType(TokenType.KW_IF); // if
            var exp = parser.ParseExpression(lexer);
            expressions.Add(exp);
            lexer.NextTokenOfType(TokenType.KW_THEN); // then
            var block = parser.ParseBlock(lexer);
            blocks.Add(block);

            while(lexer.LookAhead() == TokenType.KW_ELSEIF)
            {
                lexer.NextToken();

                exp = parser.ParseExpression(lexer);
                expressions.Add(exp);
                lexer.NextTokenOfType(TokenType.KW_THEN); // then
                block = parser.ParseBlock(lexer);
                blocks.Add(block);
            }

            if (lexer.LookAhead() == TokenType.KW_ELSE)
            {
                lexer.NextToken();

                exp = parser.ParseExpression(lexer);
                expressions.Add(exp);
                block = parser.ParseBlock(lexer);
                blocks.Add(block);
            }

            lexer.NextTokenOfType(TokenType.KW_END); // end
            return new IfStatement(expressions, blocks);
        }
    }

    // for
    class ForStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            var forToken = lexer.NextTokenOfType(TokenType.KW_FOR); // for
            var nameToken = lexer.NextIdentifier();

            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                return FinishForNum(lexer, parser, forToken.line, nameToken.text);
            }

            return FinishForIn(lexer, parser, nameToken.text);
        }

        // for i=init, limit, step do
        //
        // end
        private Statement FinishForNum(Lexer lexer, Parser parser, int lineOfFor, string varname)
        {
            lexer.NextTokenOfType(TokenType.OP_ASSIGN); // '='
            var initExp = parser.ParseExpression(lexer);
            lexer.NextTokenOfType(TokenType.SEP_COMMA); // ','
            var limitExp = parser.ParseExpression(lexer);

            Expression stepExp = null;
            if (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                stepExp = parser.ParseExpression(lexer);
            }
            else
            {
                stepExp = new IntegerExpression(lexer.line, 1);
            }

            var doToken = lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = parser.ParseBlock(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new ForNumStatement(lineOfFor, 
                                       doToken.line, 
                                       varname, 
                                       initExp, 
                                       limitExp, 
                                       stepExp, 
                                       block);
        }

        // for in do
        //
        // end
        private Statement FinishForIn(Lexer lexer, Parser parser, string name)
        {
            var names = FinishNameList(lexer, name);
            lexer.NextTokenOfType(TokenType.KW_IN); // in
            var exps = parser.ParseExpressions(lexer);
            var doToken = lexer.NextTokenOfType(TokenType.KW_DO); // do
            var block = parser.ParseBlock(lexer);
            lexer.NextTokenOfType(TokenType.KW_END); // end

            return new ForInStatement(doToken.line, names, exps, block);
        }
    }

    // 局部变量和函数定义
    class LocalAssignOrFunctionDefineStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_LOCAL); // local
            if (lexer.LookAhead() == TokenType.KW_FUNCTION)
            {
                return FinishLocalFunctionDefineStatement(lexer, parser);
            }

            return FinishLocalVariateDefineStatement(lexer, parser);
        }

        // 局部函数定义
        private Statement FinishLocalFunctionDefineStatement(Lexer lexer, Parser parser)
        {
            lexer.NextTokenOfType(TokenType.KW_FUNCTION);
            var token = lexer.NextIdentifier();
            var expression = parser.ParseFunctionDefineExpression(lexer);

            return new LocalFunctionDefineStatement(token.text, expression);
        }

        // 局部变量定义
        private Statement FinishLocalVariateDefineStatement(Lexer lexer, Parser parser)
        {
            var token = lexer.NextIdentifier();
            var names = FinishNameList(lexer, token.text);

            List<Expression> exps = null;
            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                lexer.NextToken();
                exps = parser.ParseExpressions(lexer);
            }

            return new LocalVariateStatement(lexer.line, names, exps);
        }
    }

    // 赋值和函数调用
    class AssignOrFunctionCallStatementParser : IBaseParser
    {
        public override Statement Parse(Lexer lexer, Parser parser)
        {
            var prefix = parser.ParsePrefixExpression(lexer);
            if (prefix.GetType() == typeof(FunctionCallExpression))
            {
                var exp = prefix as FunctionCallExpression;
                return new FunctionCallStatement(exp);
            }

            return ParseAssignStatement(lexer);
        }

        // 
        private Statement ParseAssignStatement(Lexer lexer)
        {
            return null;
        }
    }

    // 非局部函数定义
}
