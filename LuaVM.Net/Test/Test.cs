using System;
using System.IO;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    static class Test
    {
        public static string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        public static Prototype LoadLuac(string filename)
        {
            var rootpath = GetProjectPath();
            filename = Path.Combine(rootpath, filename);

            var bytes = File.ReadAllBytes(filename);
            var proto = Chunk.Undump(bytes);

            return proto;
        }

        public static void RunLuac(Prototype proto)
        {
            var ls = new LuaState();
            ls.Load(proto);
            ls.Call(0, 0);
        }

        public static void PrintState(LuaState ls)
        {
            var top = ls.GetTop();
            for (int i = 1; i <= top; i++)
            {
                var type = ls.Type(i);
                switch (type)
                {
                    case LuaType.LUA_TBOOLEAN:
                        Console.Write($"[{ls.ToBoolean(i)}]");
                        break;
                    case LuaType.LUA_TNUMBER:
                        Console.Write($"[{ls.ToNumber(i)}]");
                        break;
                    case LuaType.LUA_TSTRING:
                        Console.Write($"[{ls.ToString(i)}]");
                        break;
                    default:
                        Console.Write($"[{ls.TypeName(type)}]");
                        break;
                }
            }
            Console.WriteLine();
        }
    }
}
