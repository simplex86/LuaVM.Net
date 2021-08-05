using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    // 变量
    static class VariateUtils
    {
        // 获取变量名字列表
        public static List<string> GetNames(Lexer lexer, string name)
        {
            List<string> names = new List<string>();
            names.Add(name);

            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                var token = lexer.NextIdentifier();
                names.Add(token.text);
            }

            return names;
        }
    }
}
