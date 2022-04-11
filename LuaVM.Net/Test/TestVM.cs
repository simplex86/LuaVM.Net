using System;
using System.IO;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestVM
    {
        // 加载lua字节码文件
        public void Do(string filename)
        {
            var rootpath = GetProjectPath();
            filename = Path.Combine(rootpath, filename);

            Console.WriteLine("vm test:");
            var bytes = File.ReadAllBytes(filename);
            var proto = Chunk.Undump(bytes);
            RunLuac(proto);
            Console.WriteLine("vm test done!");
        }

        // 获取工程路径
        private string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        private void RunLuac(Prototype proto)
        {
            var ls = new LuaState(proto.maxStackSize + 8, proto);
            ls.SetTop(proto.maxStackSize);

            while (true)
            {
                var pc = ls.PC();
                var inst = new Instruction(ls.Fetch());
                var code = inst.OpCode();

                if (code == OperationCodes.OP_RETURN)
                {
                    break;
                }
                
                inst.Execute(ls);
                Console.Write("[{0:D2}] {1}\t", pc + 1, inst.OpName());
                PrintState(ls);
            }
        }

        private void PrintState(LuaState ls)
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
