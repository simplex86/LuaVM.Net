using System;
using System.Collections.Generic;
using LuaVM.Net.Core.Internal;

namespace LuaVM.Net.Core
{
    public class Upvalue
    {
        internal LuaValue value;

        internal Upvalue(LuaValue value)
        {
            this.value = value;
        }
    }

    public class Closure
    {
        internal Prototype proto { get; private set; }
        internal CSharpFunction func { get; private set; }
        internal Upvalue[] upvalues;

        // 由Lua字节码构造闭包
        public Closure(Prototype proto)
        {
            this.proto = proto;

            if (proto.upvalues != null && proto.upvalues.Length > 0)
            {
                upvalues = new Upvalue[proto.upvalues.Length];
            }
        }

        // 由CS函数构造闭包
        public Closure(CSharpFunction func, int n)
        {
            this.func = func;

            if (n > 0)
            {
                upvalues = new Upvalue[n];
            }
        }
    }
}
