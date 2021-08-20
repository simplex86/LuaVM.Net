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
            Console.WriteLine("vm test:");
            var proto = Test.LoadLuac(filename);
            Test.RunLuac(proto);
            Console.WriteLine("vm test done!");
        }
    }
}
