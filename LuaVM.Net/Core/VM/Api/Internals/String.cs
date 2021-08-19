using System;

namespace LuaVM.Net.Core
{
    internal static class String
    {
        internal static int Len(LuaValue value)
        {
            var s = value.GetString();
            return s.Length;
        }

        internal static LuaValue Concat(LuaValue a, LuaValue b)
        {
            var s = LuaValue.ToString(a).Item1 + LuaValue.ToString(b).Item1;
            return new LuaValue(s);
        }
    }
}
