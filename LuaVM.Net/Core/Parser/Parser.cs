using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    public class Parser
    {
        // 解析Block/Chunk
        public Block Parse(Lexer lexer)
        {
            Block block = new Block();

            ParseStatements(lexer, block);
            ParseReturnExpressions(lexer, block);
            block.lastline = lexer.line;

            return block;
        }

        // 解析语句
        private List<Statement> ParseStatements(Lexer lexer, Block block)
        {
            List<Statement> stats = new List<Statement>(4);

            var type = lexer.LookAhead();
            while (!IsReturnOrBlockEnds(type))
            {
                var parser = ParserFactory.Instance.Get(type);
                var stat = parser.Parse(lexer, this);
                if (stat.GetType() != typeof(EmptyStatement))
                {
                    stats.Add(stat);
                }

                type = lexer.LookAhead();
            }

            return stats;
        }

        // 解析表达式
        private void ParseReturnExpressions(Lexer lexer, Block block)
        {

        }

        // 是否是return或者block结尾
        private bool IsReturnOrBlockEnds(int tokenType)
        {
            return (tokenType == TokenType.KW_RETURN ||
                    tokenType == TokenType.EOF       ||
                    tokenType == TokenType.KW_END    ||
                    tokenType == TokenType.KW_ELSE   ||
                    tokenType == TokenType.KW_ELSEIF ||
                    tokenType == TokenType.KW_UNTIL);
        }
    }
}
