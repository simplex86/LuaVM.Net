using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string lua  = "--[[\n";
                   lua += "    print\n";
                   lua += "]]\n";
                   lua += "print(\"hello world\")";

            var lexer = new Lexer("test.lua", lua, 1);
            while (true)
            {
                var token = lexer.NextToken();
                Console.WriteLine($"token: {token.text}, {token.type}, {token.line}");

                if (token.type == TokenType.EOF)
                    break;
            }

            Console.ReadKey();
        }
    }
}
