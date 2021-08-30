using System;

namespace LuaVM.Net
{
    class TestClosure
    {
        // 加载lua字节码文件
        public void Do(string filename)
        {
            Console.WriteLine("closure test:");
            var proto = Test.LoadLuac(filename);
            Test.RunLuac(proto);
            Console.WriteLine("closure test done!");
        }
    }
}
