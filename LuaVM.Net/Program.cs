using System;
using System.IO;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootpath = GetProjectPath();
            LoadFile(Path.Combine(rootpath, @"Luas\01.lua"));
            // pause
            Console.ReadKey();
        }

        // 加载lua文件
        static void LoadFile(string fullname)
        {
            string lua = File.ReadAllText(fullname);
            if (string.IsNullOrEmpty(lua))
            {
                Console.WriteLine("Warning: file is empty");
                return;
            }

            var lexer = new Lexer(fullname, lua, 1);
            while (true)
            {
                var token = lexer.NextToken();
                Console.WriteLine($"token: {token.text}, {token.type}, {token.line}");

                if (token.type == TokenType.EOF)
                    break;
            }
        }

        // 获取工程路径
        static string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }
    }
}
