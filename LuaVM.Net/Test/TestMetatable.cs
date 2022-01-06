using System;

namespace LuaVM.Net
{
    class TestMetatable
    {
        // 加载lua字节码文件
        public void Do(string filename)
        {
            Console.WriteLine("metatable test:");
            var proto = Test.LoadLuac(filename);
            Test.RunLuac(proto);
            Console.WriteLine("metatable test done!");
        }
    }
}
