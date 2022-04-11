using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    class Closure
    {
        public Prototype proto { get; set; }

        public Closure(Prototype proto)
        {
            this.proto = proto;
        }
    }
}
