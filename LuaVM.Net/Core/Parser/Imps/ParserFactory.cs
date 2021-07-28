using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    class ParserFactory
    {
        private Dictionary<int, IBaseParser> dict = new Dictionary<int, IBaseParser>() {
            { TokenType.SEP_SEMI,   new EmptyStatementParser() },
            { TokenType.KW_BREAK,   new BreakStatementParser() },
            { TokenType.SEP_LABEL,  new LabelStatementParser() },
            { TokenType.KW_GOTO,    new GotoStatementParser()  },
        };        

        // 单例
        public static ParserFactory Instance { get; } = new ParserFactory();

        // 根据token类型获取对应的parser
        public IBaseParser Get(int type)
        {
            if (!dict.ContainsKey(type))
            {
                return null;
            }

            return dict[type];
        }

        // 禁止实例化该类
        protected ParserFactory()
        {

        }
    }
}
