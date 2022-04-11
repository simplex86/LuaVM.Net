using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LuaVM.Net.Core
{
    // 用最小的内存表达基础（值类型的）数据
    // 类似于C++的Union类型
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct UValue
    {
        [FieldOffset(0)]
        public bool b;
        [FieldOffset(0)]
        public long i;
        [FieldOffset(0)]
        public double f;
    }

    // 复合的数据类型，包括基础类型（值类型，u）、字符串、table、函数引用类型（o）等
    internal struct CValue
    {
        public UValue u;
        public object o;

        public void Set(bool b)
        {
            u.b = b;
        }

        public void Set(long n)
        {
            u.i = n;
        }

        public void Set(double n)
        {
            u.f = n;
        }

        public void Set(string s)
        {
            o = s;
        }

        public void Set(Closure c)
        {
            o = c;
        }

        public void Set(LuaTable t)
        {
            o = t;
        }
    }

    // 数据（值 + 类型）
    internal class TValue
    {
        public CValue v = new CValue();
        public int t = LuaType.LUA_TNONE;
    }

    // lua的数据定义
    internal class LuaValue
    {
        private TValue value = null;

        // 类型的组成
        // 0-3：主类型
        // 4-5：子类型（整数、小数；短字符串，长字符串；等等）
        // 6~N：是否可以被GC
        private const int LUA_TYPE_MASK             = 0x3F; // 类型位模板
        private const int LUA_LUATYPE_MASK          = 0x0F; // 主类型位模板
        private const int LUA_SUBTYPE_MASK          = 0x30; // 子类型位模板
        private const int LUA_SUBTYPE_INTEGER_NUM   = 0x10; // 整数
        private const int LUA_SUBTYPE_FLOAT_NUM     = 0x20; // 小数
        private const int LUA_SUBTYPE_SHORT_STR     = 0x10; // 短字符串
        private const int LUA_SUBTYPE_LONG_STR      = 0x20; // 长字符串

        // 获取类型
        internal static int GetType(LuaValue value)
        {
            if (value == null)
            {
                return LuaType.LUA_TNIL;
            }

            return value.type;
        }

        internal static int GetHash(LuaValue value)
        {
            if (value.IsBoolean())
            {
                return Hash.Get(value.GetBoolean());
            }
            if (value.IsInteger())
            {
                return Hash.Get(value.GetInteger());
            }
            if (value.IsFloat())
            {
                return Hash.Get(value.GetFloat());
            }
            if (value.IsString())
            {
                return Hash.Get(value.GetString());
            }

            return 0;
        }

        // nil
        public LuaValue()
        {
            value = new TValue
            {
                t = LuaType.LUA_TNIL
            };
        }

        public LuaValue(bool b)
        {
            Set(b);
        }

        public LuaValue(long n)
        {
            Set(n);
        }

        public LuaValue(double n)
        {
            Set(n);
        }

        public LuaValue(string s)
        {
            Set(s);
        }

        public LuaValue(Closure c)
        {
            Set(c);
        }

        public LuaValue(LuaTable t)
        {
            Set(t);
        }

        // 获取数据类型
        public int type
        {
            get { return (value == null) ? LuaType.LUA_TNIL : value.t & LUA_LUATYPE_MASK; }
        }

        // 设值
        public void Set(bool b)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(b);
            value.t = LuaType.LUA_TBOOLEAN;
        }

        // 设值
        public void Set(long n)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(n);
            value.t = LuaType.LUA_TNUMBER | LUA_SUBTYPE_INTEGER_NUM;
        }

        // 设值
        public void Set(double n)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(n);
            value.t = LuaType.LUA_TNUMBER | LUA_SUBTYPE_FLOAT_NUM;
        }

        // 设值
        public void Set(string s)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(s);
            value.t = LuaType.LUA_TSTRING;
        }

        // 设值
        public void Set(Closure c)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(c);
            value.t = LuaType.LUA_TFUNCTION;
        }

        // 设值
        public void Set(LuaTable t)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(t);
            value.t = LuaType.LUA_TTABLE;
        }

        public bool GetBoolean()
        {
            return u.b;
        }

        public long GetInteger()
        {
            return u.i;
        }

        public double GetFloat()
        {
            return u.f;
        }

        public string GetString()
        {
            return o as string;
        }

        public Closure GetFunction()
        {
            return o as Closure;
        }

        public LuaTable GetTable()
        {
            return o as LuaTable;
        }

        // 是否为nil
        public bool IsNil()
        {
            if (value == null)
            {
                return true;
            }

            return type == LuaType.LUA_TNIL;
        }

        // 是否为布尔类型
        public bool IsBoolean()
        {
            if (value == null)
            {
                return false;
            }

            return type == LuaType.LUA_TBOOLEAN;
        }

        // 是否为数字（整数或小数）
        public bool IsNumber()
        {
            if (value == null)
            {
                return false;
            }

            return type == LuaType.LUA_TNUMBER;
        }

        // 是否为整数
        public bool IsInteger()
        {
            if (!IsNumber())
            {
                return false;
            }

            return (value.t & LUA_SUBTYPE_MASK) == LUA_SUBTYPE_INTEGER_NUM;
        }

        // 是否为小数
        public bool IsFloat()
        {
            if (!IsNumber())
            {
                return false;
            }

            return (value.t & LUA_SUBTYPE_MASK) == LUA_SUBTYPE_FLOAT_NUM;
        }

        // 是否为字符串
        public bool IsString()
        {
            if (value == null)
            {
                return false;
            }

            return type == LuaType.LUA_TSTRING;
        }

        // 是否为函数
        public bool IsFunction()
        {
            if (value == null)
            {
                return false;
            }

            return type == LuaType.LUA_TFUNCTION;
        }

        // 是否为table
        public bool IsTable()
        {
            if (value == null)
            {
                return false;
            }

            return type == LuaType.LUA_TTABLE;
        }

        public static Tuple<long, bool> ToInteger(LuaValue value)
        {
            if (value.IsInteger())
            {
                var n = value.GetInteger();
                return Tuple.Create(n, true);
            }
            if (value.IsFloat())
            {
                var n = (long)value.GetFloat();
                return Tuple.Create(n, true);
            }

            if (value.IsString())
            {
                var s = value.GetString();
                if (long.TryParse(s, out long n))
                {
                    return Tuple.Create(n, true);
                }
            }

            return Tuple.Create(0L, false);
        }

        public static Tuple<double, bool> ToFloat(LuaValue value)
        {
            if (value.IsInteger())
            {
                var n = (double)value.GetInteger();
                return Tuple.Create(n, true);
            }
            if (value.IsFloat())
            {
                var n = value.GetFloat();
                return Tuple.Create(n, true);
            }

            if (value.IsString())
            {
                var s = value.GetString();
                if (double.TryParse(s, out double n))
                {
                    return Tuple.Create(n, true);
                }
            }

            return Tuple.Create(0.0, false);
        }

        public static Tuple<string, bool> ToString(LuaValue value)
        {
            if (value.IsString())
            {
                var s = value.GetString();
                 return Tuple.Create(s, true);
            }

            if (value.IsInteger())
            {
                var s = value.GetInteger().ToString();
                return Tuple.Create(s, true);
            }
            if (value.IsFloat())
            {
                var s = value.GetFloat().ToString();
                return Tuple.Create(s, true);
            }

            return Tuple.Create("", false);
        }

        private UValue u
        {
            get { return value.v.u; }
        }

        private object o
        {
            get { return value.v.o; }
        }
    }
}
