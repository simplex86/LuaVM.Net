using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class Closure
    {
        internal Prototype proto { get; private set; } = null;
        internal Upvalue[] upvalues { get; private set; } = null;
        internal CSharpFunction cfunc { get; private set; } = null;

        internal Closure(Prototype proto)
        {
            this.proto = proto;

            var len = proto.upvalues.Length;
            InitUpvalues(len);
        }

        internal Closure(CSharpFunction func)
        {
            this.cfunc = func;
            InitUpvalues(0);
        }

        private void InitUpvalues(int len)
        {
            if (len > 0)
            {
                upvalues = new Upvalue[len];
            }
        }
    }

    internal class Upvalue
    {
        internal LuaValue value { get; set; }

        internal Upvalue(LuaValue value)
        {
            this.value = value;
        }
    }

    public abstract class CSharpFunction
    {
        internal abstract int Invoke(LuaState ls);
    }
}
