using System;

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

    internal static class Methods
    {
        internal static CSharpFunction Print
        {
            get { return new Print(); }
        }
    }
}
