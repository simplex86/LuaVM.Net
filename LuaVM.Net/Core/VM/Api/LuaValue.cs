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
        public int t = ValueType.LUA_TNONE;
    }

    // lua的数据定义
    internal class LuaValue
    {
        private TValue value = null;

        // 获取类型
        public static int GetType(LuaValue value)
        {
            return value.type;
        }

        public LuaValue()
        {

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
            get { return (value == null) ? ValueType.LUA_TNIL : value.t; }
        }

        // 设值
        public void Set(bool b)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(b);
            value.t = ValueType.LUA_TBOOLEAN;
        }

        // 设值
        public void Set(long n)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(n);
            value.t = ValueType.LUA_TNUMBER;
        }

        // 设值
        public void Set(double n)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(n);
            value.t = ValueType.LUA_TNUMBER;
        }

        // 设值
        public void Set(string s)
        {
            if (value == null)
            {
                value = new TValue();
            }
            value.v.Set(s);
            value.t = ValueType.LUA_TSTRING;
        }
    }
}
