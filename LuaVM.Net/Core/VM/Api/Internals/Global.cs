using System;

namespace LuaVM.Net.Core
{
    class Global
    {
        internal static int lua_Print(LuaState ls)
        {
            string text = "";

            var nArgs = ls.GetTop();
            for (var i = 1; i <= nArgs; i++)
            {
                if (ls.IsNil(i))
                {
                    text += "nil";
                }
                else if (ls.IsString(i))
                {
                    text += ls.ToString(i);
                }
                else if (ls.IsBoolean(i))
                {
                    text += ls.ToBoolean(i).ToString();
                }
                else if (ls.IsInteger(i))
                {
                    text += ls.ToInteger(i).ToString();
                }
                else if (ls.IsNumber(i))
                {
                    text += ls.ToNumber(i).ToString();
                }
                else
                {
                    text += ls.TypeName(ls.Type(i));
                }

                if (i < nArgs)
                {
                    text += "\t";
                }
            }
            Console.WriteLine(text);

            return 0;
        }

        internal static int lua_ToNumber(LuaState ls)
        {
            return 0;
        }

        internal static int lua_ToString(LuaState ls)
        {
            return 0;
        }
    }
}
