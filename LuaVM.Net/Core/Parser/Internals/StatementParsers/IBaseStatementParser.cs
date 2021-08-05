using System;

namespace LuaVM.Net.Core
{
    //
    interface IBaseStatementParser
    {
        //
        Statement Parse(Lexer lexer);
    }
}
