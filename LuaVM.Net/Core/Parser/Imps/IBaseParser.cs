using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    //
    abstract class IBaseParser
    {
        //
        public abstract Statement Parse(Lexer lexer, Parser parser);

        //
        protected List<string> FinishNameList(Lexer lexer, string name)
        {
            List<string> list = new List<string>();
            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                var token = lexer.NextIdentifier();
                list.Add(token.text);
            }

            return list;
        }
    }
}
