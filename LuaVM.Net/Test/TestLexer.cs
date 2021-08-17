using System;
using System.IO;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestLexer
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
            Console.WriteLine("lexer test:");
            PrintLexer(lexer);
            Console.WriteLine("lexer test done!");
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

        private void PrintLexer(Lexer lexer)
        {
            while (true)
            {
                var token = lexer.NextToken();
                Console.WriteLine($"token: {token.text}, {token.type}, {token.line}");

                if (token.type == TokenType.EOF)
                    break;
            }
        }
    }
}
