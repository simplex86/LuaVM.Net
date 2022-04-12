using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    public class Closure
    {
        public Prototype proto { get; private set; }
        public CSharpFunction func { get; private set; }

        public Closure(Prototype proto)
        {
            this.proto = proto;
        }

        public Closure(CSharpFunction func)
        {
            this.func = func;
        }
    }
}
