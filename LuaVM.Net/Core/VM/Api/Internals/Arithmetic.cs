using System;

namespace LuaVM.Net.Core
{
    internal static class Arithmetic
    {
        public static LuaValue Do(LuaValue a, LuaValue b, int op, LuaState ls)
        {
            var oper = operators[op];
            return Do(a, b, oper, ls);
        }

        public static bool DoMetamethod(LuaValue a, LuaValue b, int op, LuaState ls)
        {
            var mm = operators[op].metamethod;
            var rt = LuaState.CallMetamethod(a, b, mm, ls);

            if (rt.Item2)
            {
                ls.stack.Push(rt.Item1);
            }

            return rt.Item2;
        }

        private class Operator
        {
            public string metamethod;
            public Func<long, long, long> integerFunc;
            public Func<double, double, double> floatFunc;

            public Operator(string metamethod, 
                            Func<long, long, long> integerFunc, 
                            Func<double, double, double> floatFunc)
            {
                this.metamethod = metamethod;
                this.integerFunc = integerFunc;
                this.floatFunc = floatFunc;
            }
        }

        private static Operator[] operators = new Operator[] {
            new Operator("__add",  iadd,  fadd),
            new Operator("__sub",  isub,  fsub),
            new Operator("__mul",  imul,  fmul),
            new Operator("__mod",  imod,  fmod),
            new Operator("__pow",  null,  pow),
            new Operator("__div",  null,  div),
            new Operator("__idiv", iidiv, fidiv),
            new Operator("__band", band,  null),
            new Operator("__bor",  bor,   null),
            new Operator("__bxor", bxor,  null),
            new Operator("__shl",  shl,   null),
            new Operator("__shr",  shr,   null),
            new Operator("__num",  inum,  fnum),
            new Operator("__bnot", bnot,  null),
        };

        private static long iadd(long a, long b)
        {
            return a + b;
        }

        private static double fadd(double a, double b)
        {
            return a + b;
        }

        private static long isub(long a, long b)
        {
            return a - b;
        }

        private static double fsub(double a, double b)
        {
            return a - b;
        }

        private static long imul(long a, long b)
        {
            return a * b;
        }

        private static double fmul(double a, double b)
        {
            return a * b;
        }

        private static long imod(long a, long b)
        {
            return Number.IMod(a, b);
        }

        private static double fmod(double a, double b)
        {
            return Number.FMod(a, b);
        }

        private static double pow(double a, double b)
        {
            return Math.Pow(a, b);
        }

        private static double div(double a, double b)
        {
            return a / b;
        }

        private static long iidiv(long a, long b)
        {
            return Number.IFloorDiv(a, b);
        }

        private static double fidiv(double a, double b)
        {
            return Number.FFloorDiv(a, b);
        }

        private static long band(long a, long b)
        {
            return a & b;
        }

        private static long bor(long a, long b)
        {
            return a | b;
        }

        private static long bxor(long a, long b)
        {
            return a ^ b;
        }

        private static long shl(long a, long b)
        {
            return Number.ShiftLeft(a, b);
        }

        private static long shr(long a, long b)
        {
            return Number.ShiftRight(a, b);
        }

        private static long inum(long a, long _)
        {
            return -a;
        }

        private static double fnum(double a, double _)
        {
            return -a;
        }

        private static long bnot(long a, long _)
        {
            return ~a;
        }

        private static LuaValue Do(LuaValue a, LuaValue b, Operator op, LuaState ls)
        {
            if (op.floatFunc == null)
            {
                Tuple<long, bool> v1 = LuaValue.ToInteger(a);
                Tuple<long, bool> v2 = LuaValue.ToInteger(b);
                if (v1.Item2 && v2.Item2)
                {
                    var n = op.integerFunc(v1.Item1, v2.Item1);
                    return new LuaValue(n);
                }
            }
            else
            {
                if (op.integerFunc != null)
                {
                    if (a.IsInteger() && b.IsInteger())
                    {
                        var x = a.GetInteger();
                        var y = b.GetInteger();
                        var n = op.integerFunc(x, y);
                        return new LuaValue(n);
                    }
                }

                var v1 = LuaValue.ToFloat(a);
                var v2 = LuaValue.ToFloat(b);
                if (v1.Item2 && v2.Item2)
                {
                    var n = op.floatFunc(v1.Item1, v2.Item1);
                    return new LuaValue(n);
                }
            }            

            return null;
        }
    }
}
