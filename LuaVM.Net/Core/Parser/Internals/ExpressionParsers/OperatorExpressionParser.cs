using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 运算符表达式解析器
    class OperatorExpressionParser : IBaseExpressionParser
    {
        private CommonExpressionParser cexpParser = new CommonExpressionParser();

        public Expression Parse(Lexer lexer)
        {
            return ParseOrExpression(lexer);
        }

        // or
        private Expression ParseOrExpression(Lexer lexer)
        {
            var exp = ParseAndExpression(lexer);
            while (lexer.LookAhead() == TokenType.OP_OR)
            {
                var token = lexer.NextToken();
                var exp2 = ParseAndExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }
            return exp;
        }

        // and
        private Expression ParseAndExpression(Lexer lexer)
        {
            var exp = ParseCompareExpression(lexer);
            while (lexer.LookAhead() == TokenType.OP_AND)
            {
                var token = lexer.NextToken();
                var exp2 = ParseCompareExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }
            return exp;
        }

        // compare
        private Expression ParseCompareExpression(Lexer lexer)
        {
            var exp = ParseBitOrExpression(lexer);
            while (true)
            {
                var type = lexer.LookAhead();
                if (type == TokenType.OP_LT ||
                    type == TokenType.OP_GT ||
                    type == TokenType.OP_NE ||
                    type == TokenType.OP_LE ||
                    type == TokenType.OP_GE ||
                    type == TokenType.OP_EQ)
                {
                    var token = lexer.NextToken();
                    var exp2 = ParseBitOrExpression(lexer);
                    exp = new BinOpExpression(token.line, token.type, exp, exp2);
                }
                else
                {
                    return exp;
                }
            }
            return exp;
        }

        // x | y
        private Expression ParseBitOrExpression(Lexer lexer)
        {
            var exp = ParseBitXorExpression(lexer);
            while (lexer.LookAhead() == TokenType.OP_AND)
            {
                var token = lexer.NextToken();
                var exp2 = ParseBitXorExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }
            return exp;
        }

        // x ~ y
        private Expression ParseBitXorExpression(Lexer lexer)
        {
            var exp = ParseBitAndExpression(lexer);
            while (lexer.LookAhead() == TokenType.OP_AND)
            {
                var token = lexer.NextToken();
                var exp2 = ParseBitAndExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }
            return exp;
        }

        // x & y
        private Expression ParseBitAndExpression(Lexer lexer)
        {
            var exp = ParseBitShiftExpression(lexer);
            while (lexer.LookAhead() == TokenType.OP_AND)
            {
                var token = lexer.NextToken();
                var exp2 = ParseBitShiftExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }
            return exp;
        }

        // shift
        private Expression ParseBitShiftExpression(Lexer lexer)
        {
            var exp = ParseConcatExpression(lexer);
            while (true)
            {
                var type = lexer.LookAhead();
                if (type == TokenType.OP_BIT_SHL ||
                    type == TokenType.OP_BIT_SHR)
                {
                    var token = lexer.NextToken();
                    var exp2 = ParseConcatExpression(lexer);
                    exp = new BinOpExpression(token.line, token.type, exp, exp2);
                }
                else
                {
                    return exp;
                }
            }
            return exp;
        }

        // 字符串连接符 '..'
        private Expression ParseConcatExpression(Lexer lexer)
        {
            var exp = ParseAddExpression(lexer);
            if (lexer.LookAhead() != TokenType.OP_CONCAT)
            {
                return exp;
            }

            var line = 0;
            List<Expression> exps = new List<Expression>();
            exps.Add(exp);

            while (lexer.LookAhead() == TokenType.OP_CONCAT)
            {
                var token = lexer.NextToken();
                line = token.line;
                exp = ParseAddExpression(lexer);
                exps.Add(exp);
            }

            return new ConcatExpression(line, exps);
        }

        // +, -
        private Expression ParseAddExpression(Lexer lexer)
        {
            var exp = ParseMulExpression(lexer);
            while (true)
            {
                var type = lexer.LookAhead();
                if (type == TokenType.OP_ADD ||
                    type == TokenType.OP_SUB)
                {
                    var token = lexer.NextToken();
                    var exp2 = ParseMulExpression(lexer);
                    exp = new BinOpExpression(token.line, token.type, exp, exp2);
                }
                else
                {
                    return exp;
                }
            }
            return exp;
        }

        // *, /, //, %
        private Expression ParseMulExpression(Lexer lexer)
        {
            var exp = ParseUnaryExpression(lexer);
            while (true)
            {
                var type = lexer.LookAhead();
                if (type == TokenType.OP_MUL ||
                    type == TokenType.OP_DIV ||
                    type == TokenType.OP_MOD ||
                    type == TokenType.OP_IDIV)
                {
                    var token = lexer.NextToken();
                    var exp2 = ParseUnaryExpression(lexer);
                    exp = new BinOpExpression(token.line, token.type, exp, exp2);
                }
                else
                {
                    return exp;
                }
            }
            return exp;
        }

        // unary
        private Expression ParseUnaryExpression(Lexer lexer)
        {
            var type = lexer.LookAhead();
            if (type == TokenType.OP_UNM     ||
                type == TokenType.OP_BIT_NOT ||
                type == TokenType.OP_LEN     ||
                type == TokenType.OP_NOT)
            {
                var token = lexer.NextToken();
                var exp = ParseUnaryExpression(lexer);
                return new UnaryOpExpression(token.line, token.type, exp);
            }

            return ParsePowExpression(lexer);
        }

        // pow
        private Expression ParsePowExpression(Lexer lexer)
        {
            var exp = cexpParser.Parse(lexer);
            if (lexer.LookAhead() == TokenType.OP_POW)
            {
                var token = lexer.NextToken();
                var exp2 = ParseUnaryExpression(lexer);
                exp = new BinOpExpression(token.line, token.type, exp, exp2);
            }

            return exp;
        }
    }
}
