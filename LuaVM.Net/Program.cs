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
            LoadFile(Path.Combine(rootpath, @"Luas\03.lua"));
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

            TestParser(fullname, lua);
            Console.WriteLine("done!");
        }

        // 获取工程路径
        static string GetProjectPath()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        static void TestLexer(string filename, string lua)
        {
            var lexer = new Lexer(filename, lua, 1);
            Console.WriteLine("lexer:");
            while (true)
            {
                var token = lexer.NextToken();
                Console.WriteLine($"token: {token.text}, {token.type}, {token.line}");

                if (token.type == TokenType.EOF)
                    break;
            }
            Console.WriteLine("lexer done!");
        }

        static void TestParser(string filename, string lua)
        {
            var lexer = new Lexer(filename, lua, 1);
            var parser = new Parser();

            Console.WriteLine("parser:");
            var block = parser.Parse(lexer);
            Console.WriteLine("parser done!");
        }
    }
}
