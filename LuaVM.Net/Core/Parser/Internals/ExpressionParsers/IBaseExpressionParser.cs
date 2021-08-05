using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    interface IBaseExpressionParser
    {
        Expression Parse(Lexer lexr);
    }
}
