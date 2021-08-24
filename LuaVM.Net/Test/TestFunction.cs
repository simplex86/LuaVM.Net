using System;

namespace LuaVM.Net
{
    class TestFunction
    {
        // 加载lua字节码文件
        public void Do(string filename)
        {
            Console.WriteLine("function test:");
            var proto = Test.LoadLuac(filename);
            Test.RunLuac(proto);
            Console.WriteLine("function test done!");
        }
    }
}
