using System;
using System.IO;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestParser
    {
        public void Do(string filename)
        {
            var rootpath = GetProjectPath();
            filename = Path.Combine(rootpath, filename);

            string lua = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(lua))
            {
                Console.WriteLine("Warning: file is empty");
                return;
            }

            var lexer = new Lexer(filename, lua, 1);
            var parser = new Parser();

            Console.WriteLine("parser test:");
            var block = parser.Parse(lexer);
            PrintBlock(block);
            Console.WriteLine("parser test done!");
        }

        // 获取工程路径
        private string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        // 加载lua文件
        private void Load(string fullname)
        {
            string lua = File.ReadAllText(fullname);
            if (string.IsNullOrEmpty(lua))
            {
                Console.WriteLine("Warning: file is empty");
                return;
            }
        }

        void PrintBlock(Block block)
        {
            var json = ToJson(block);
            Console.WriteLine(json);
        }

        private string ToJson(object o)
        {
            LitJson.JsonWriter jw = new LitJson.JsonWriter
            {
                PrettyPrint = true
            };
            LitJson.JsonMapper.ToJson(o, jw);

            return jw.TextWriter.ToString();
        }
    }
}
