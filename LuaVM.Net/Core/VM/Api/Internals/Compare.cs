using System;

namespace LuaVM.Net.Core
{
    internal static class Compare
    {
        //
        internal static bool Do(int idx1, int idx2, int op, LuaState ls)
        {
            var stack = ls.stack;

            if (!stack.IsValid(idx1) || !stack.IsValid(idx2))
            {
                return false;
            }

            var a = stack.Get(idx1);
            var b = stack.Get(idx2);

            switch (op)
            {
                case Operations.LUA_OPEQ: return _eq(a, b, ls);
                case Operations.LUA_OPLT: return _lt(a, b, ls);
                case Operations.LUA_OPLE: return _le(a, b, ls);
                default: Error.Commit("invalid compare op!"); return false;
            }
        }

        public static bool RawEqual(int idx1, int idx2, LuaState ls)
        {
            var stack = ls.stack;

            if (!stack.IsValid(idx1) || !stack.IsValid(idx2))
            {
                return false;
            }

            var a = stack.Get(idx1);
            var b = stack.Get(idx2);

            return _eq(a, b, null);
        }

        // 相等
        private static bool _eq(LuaValue a, LuaValue b, LuaState ls)
        {
            if (LuaValue.GetType(a) == LuaType.LUA_TNIL)
            {
                return LuaValue.GetType(b) == LuaType.LUA_TNIL;
            }
            if (LuaValue.GetType(b) == LuaType.LUA_TNIL)
            {
                return false;
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
            if (a.IsTable())
            {
                if (b.IsTable() && a != b)
                {
                    var r = LuaState.CallMetamethod(a, b, "__eq", ls);
                    if (r.Item2)
                    {
                        return r.Item1.GetBoolean();
                    }
                }

                return a == b;
            }

            return false;
        }

        // 小于
        private static bool _lt(LuaValue a, LuaValue b, LuaState ls)
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

            var r = LuaState.CallMetamethod(a, b, "__lt", ls);
            if (r.Item2)
            {
                return r.Item1.GetBoolean();
            }

            Error.Commit("comparison error!");
            return false;
        }

        // 小于或等于
        private static bool _le(LuaValue a, LuaValue b, LuaState ls)
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

            var r1 = LuaState.CallMetamethod(a, b, "__eq", ls);
            if (r1.Item2)
            {
                return r1.Item1.GetBoolean();
            }
            var r2 = LuaState.CallMetamethod(a, b, "__lt", ls);
            if (r2.Item2)
            {
                return r2.Item1.GetBoolean();
            }

            Error.Commit("comparison error!");
            return false;
        }
    }
}
