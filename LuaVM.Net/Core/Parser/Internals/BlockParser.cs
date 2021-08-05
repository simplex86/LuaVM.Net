using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    // block解析器
    // 必须保证该类是无状态的
    class BlockParser
    {
        // 解析并获取block数据
        public Block Parse(Lexer lexer)
        {
            var stats = ParseStatements(lexer);
            var exps = ParseReturnExpressions(lexer);

            return new Block(stats, exps, lexer.line); 
        }

        // 解析语句列表
        private List<Statement> ParseStatements(Lexer lexer)
        {
            List<Statement> stats = new List<Statement>(4);

            var type = lexer.LookAhead();
            while (!IsReturnOrBlockEnds(type))
            {
                var stat = GetParser(type).Parse(lexer);
                if (stat.GetType() != typeof(EmptyStatement))
                {
                    stats.Add(stat);
                }

                type = lexer.LookAhead();
            }

            return stats;
        }

        // 解析return表达式
        private List<Expression> ParseReturnExpressions(Lexer lexer)
        {
            if (lexer.LookAhead() != TokenType.KW_RETURN)
            {
                return null;
            }

            lexer.NextToken();
            switch (lexer.LookAhead())
            {
                case TokenType.EOF:
                case TokenType.KW_END:
                case TokenType.KW_ELSE:
                case TokenType.KW_ELSEIF:
                case TokenType.KW_UNTIL:
                    return new List<Expression>();
                case TokenType.SEP_SEMI:
                    lexer.NextToken();
                    return new List<Expression>();
                default:
                    break;
            }

            var expressions = ExpressionUtils.ParseExpressions(lexer);
            if (lexer.LookAhead() == TokenType.SEP_SEMI)
            {
                lexer.NextToken();
            }

            return expressions;
        }

        // 是否是return或者block结尾
        private bool IsReturnOrBlockEnds(int tokenType)
        {
            return tokenType == TokenType.KW_RETURN ||
                   tokenType == TokenType.EOF       ||
                   tokenType == TokenType.KW_END    ||
                   tokenType == TokenType.KW_ELSE   ||
                   tokenType == TokenType.KW_ELSEIF ||
                   tokenType == TokenType.KW_UNTIL;
        }

        // 根据token类型获取对应的parser
        private IBaseStatementParser GetParser(int type)
        {
            switch (type)
            {
                case TokenType.SEP_SEMI:    return new EmptyStatementParser();
                case TokenType.KW_BREAK:    return new BreakStatementParser();
                case TokenType.SEP_LABEL:   return new LabelStatementParser();
                case TokenType.KW_GOTO:     return new GotoStatementParser();
                case TokenType.KW_DO:       return new DoStatementParser();
                case TokenType.KW_WHILE:    return new WhileStatementParser();
                case TokenType.KW_REPEAT:   return new RepeatStatementParser();
                case TokenType.KW_IF:       return new IfStatementParser();
                case TokenType.KW_FOR:      return new ForStatementParser();
                case TokenType.KW_FUNCTION: return new FunctionDefineStatementParser();
                case TokenType.KW_LOCAL:    return new LocalAssignOrFunctionDefineStatementParser();
                default: break;
            }

            return new AssignOrFunctionCallStatementParser();
        }
    }
}
