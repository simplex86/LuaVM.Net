using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class Print : CSharpFunction
    {
        internal override int Invoke(LuaState ls)
        {
            var args = ls.GetTop();
            for (int i=1; i<=args; i++)
            {
                if (ls.IsBoolean(i))
                {
                    Console.Write(ls.ToBoolean(i).ToString());
                }
                else if (ls.IsString(i))
                {
                    Console.Write(ls.ToString(i));
                }
                else
                {
                    var type = ls.Type(i);
                    Console.Write(ls.TypeName(type));
                }

                if (i < args)
                {
                    Console.Write("\t");
                }
            }
            Console.WriteLine();

            return 0;
        }
    }

    internal class GetMetatable : CSharpFunction
    {
        internal override int Invoke(LuaState ls)
        {
            if (!ls.GetMetatable(1))
            {
                ls.Push();
            }
            return 1;
        }
    }

    internal class SetMetatable : CSharpFunction
    {
        internal override int Invoke(LuaState ls)
        {
            ls.SetMetatable(1);
            return 1;
        }
    }

    internal static class Methods
    {
        internal static Dictionary<string, CSharpFunction> dict { get; } = new Dictionary<string, CSharpFunction>() {
             { "print",         new Print() },
             { "getmetatable",  new GetMetatable() },
             { "setmetatable",  new SetMetatable() },
        };
    }
}
