using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // do
    class DoStatementParser : IBaseStatementParser
    {
        private BlockParser blockParser = new BlockParser();

        public Statement Parse(Lexer lexer)
        {
            lexer.NextTokenOfType(TokenType.KW_DO);
            Block block = blockParser.Parse(lexer);
            lexer.NextTokenOfType(TokenType.KW_END);

            return new DoStatement(block);
        }
    }
}
