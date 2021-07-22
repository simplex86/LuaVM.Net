using System;

namespace LuaVM.Net.Core
{
    public class Token
    {
        // token
        public string text { get; private set; } = "EOF";
        // token类型
        public int type { get; private set; } = TokenType.EOF;
        // 行号
        public int line { get; private set; } = 0;

        public Token(int type, string text,  int line)
        {
            this.text = text;
            this.type = type;
            this.line = line;
        }
    }
}
