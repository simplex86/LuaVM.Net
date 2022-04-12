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
            var bytes = File.ReadAllBytes(filename);
            var ls = new LuaState();
            ls.Load(bytes, "chunk", "b");
            ls.Call(0, 0);
        }

        // 获取工程路径
        private string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }
    }
}
