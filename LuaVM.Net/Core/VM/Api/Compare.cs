using System;

namespace LuaVM.Net.Core
{
    internal static class Compare
    {
        //
        internal static bool Do(LuaValue a, LuaValue b, int op)
        {
            switch (op)
            {
                case Operations.LUA_OPEQ: return _eq(a, b);
                case Operations.LUA_OPLT: return _lt(a, b);
                case Operations.LUA_OPLE: return _le(a, b);
                default: Error.Commit("invalid compare op!"); return false;
            }
        }

        // 相等
        private static bool _eq(LuaValue a, LuaValue b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (b.IsBoolean() && b.IsBoolean())
            {
                return a.GetBoolean() == b.GetBoolean();
            }
            if (a.IsString() && b.IsString())
            {
                return a.GetString().Equals(b.GetString());
            }
            if (a.IsInteger())
            {
                if (b.IsInteger())
                {
                    return a.GetInteger() == b.GetInteger();
                }
                if (b.IsFloat())
                {
                    return Math.Abs(a.GetFloat() - b.GetFloat()) <= double.Epsilon;
                }
                return false;
            }
            if (a.IsFloat())
            {
                if (b.IsInteger())
                {
                    return Math.Abs(a.GetFloat() - b.GetInteger()) <= double.Epsilon;
                }
                if (b.IsFloat())
                {
                    return Math.Abs(a.GetFloat() - b.GetFloat()) <= double.Epsilon;
                }
                return false;
            }

            return false;
        }

        // 小于
        private static bool _lt(LuaValue a, LuaValue b)
        {
            if (a.IsString() && b.IsString())
            {
                return string.Compare(a.GetString(), b.GetString(), StringComparison.Ordinal) == -1;
            }
            if (a.IsInteger())
            {
                if (b.IsInteger())
                {
                    return a.GetInteger() < b.GetInteger();
                }
                if (b.IsFloat())
                {
                    return a.GetFloat() < b.GetFloat();
                }
                return false;
            }
            if (a.IsFloat())
            {
                if (b.IsInteger())
                {
                    return a.GetFloat() < b.GetInteger();
                }
                if (b.IsFloat())
                {
                    return a.GetFloat() < b.GetFloat();
                }
                return false;
            }

            Error.Commit("comparison error!");
            return false;
        }

        // 小于或等于
        private static bool _le(LuaValue a, LuaValue b)
        {
            if (a.IsString() && b.IsString())
            {
                return string.Compare(a.GetString(), b.GetString(), StringComparison.Ordinal) <= 0;
            }
            if (a.IsInteger())
            {
                if (b.IsInteger())
                {
                    return a.GetInteger() <= b.GetInteger();
                }
                if (b.IsFloat())
                {
                    return a.GetFloat() <= b.GetFloat();
                }
                return false;
            }
            if (a.IsFloat())
            {
                if (b.IsInteger())
                {
                    return a.GetFloat() <= b.GetInteger();
                }
                if (b.IsFloat())
                {
                    return a.GetFloat() <= b.GetFloat();
                }
                return false;
            }

            Error.Commit("comparison error!");
            return false;
        }
    }
}
