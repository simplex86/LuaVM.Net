using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    interface IBaseParser
    {
        Statement Parse(Lexer lexer);
    }
}
