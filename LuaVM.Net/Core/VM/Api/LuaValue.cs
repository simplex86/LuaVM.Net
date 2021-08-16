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
        public static int GetType(LuaValue value)
        {
            return value.type;
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
