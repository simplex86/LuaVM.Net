using System;

namespace LuaVM.Net.Core
{
    internal static class Arithmetic
    {
        public static LuaValue Do(LuaValue a, LuaValue b, int op)
        {
            var oper = operators[op];
            return Do(a, b, oper);
        }

        private class Operator
        {
            public Func<long, long, long> integerFunc;
            public Func<double, double, double> floatFunc;
        }

        private static Operator[] operators = new Operator[] {
            new Operator
            {
                integerFunc = iadd,
                floatFunc = fadd
            },
            new Operator
            {
                integerFunc = isub,
                floatFunc = fsub
            },
            new Operator
            {
                integerFunc = imul,
                floatFunc = fmul
            },
            new Operator
            {
                integerFunc = imod,
                floatFunc = fmod
            },
            new Operator
            {
                integerFunc = null,
                floatFunc = pow
            },
            new Operator
            {
                integerFunc = null,
                floatFunc = div
            },
            new Operator
            {
                integerFunc = iidiv,
                floatFunc = fidiv
            },
            new Operator
            {
                integerFunc = band,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = bor,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = bxor,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = shl,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = shr,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = inum,
                floatFunc = fnum
            },
            new Operator
            {
                integerFunc = bnot,
                floatFunc = null
            }
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

        private static LuaValue Do(LuaValue a, LuaValue b, Operator op)
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
