using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal static class Table
    {
        internal static long Len(LuaValue value)
        {
            var t = value.GetTable();
            return t.Len();
        }
    }
}
