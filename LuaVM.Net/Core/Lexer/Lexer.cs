using System;
using System.Collections.Generic;
using System.Text;

namespace LuaVM.Net.Core
{
    public class Lexer
    {
        public LexerData NextLexer(string chunkname, string chunk)
        {
            var body = new LexerData(chunkname, chunk, 1);
            return body;
        }
    }
}
