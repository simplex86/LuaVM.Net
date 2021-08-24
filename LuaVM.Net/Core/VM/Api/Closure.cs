using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class Closure
    {
        internal Prototype proto { get; private set; }

        public Closure(Prototype proto)
        {
            this.proto = proto;
        }
    }
}
