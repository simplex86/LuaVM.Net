﻿using System;
using System.IO;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            TestVM();
            // pause
            Console.Write("press any key to be continue...");
            Console.ReadKey();
        }

        // 执行词法分析器的测试
        static void TestLexer()
        {
            var test = new TestLexer();
            test.Do(@"Luas\01.lua");
        }

        // 执行语法分析器的测试
        static void TestParser()
        {
            var test = new TestParser();
            test.Do(@"Luas\01.lua");
        }

        // 执行字节反编译器的测试
        static void TestUndump()
        {
            var test = new TestUndump();
            test.Do(@"Luas\03.out");
        }

        // 执行Lua数据类型的测试
        static void TestValue()
        {
            var test = new TestValue();
            test.Do();
        }

        static void TestOperations()
        {
            var test = new TestOperations();
            test.Do();
        }

        // 执行字节反编译器的测试
        static void TestVM()
        {
            var test = new TestVM();
            test.Do(@"Luas\05.out");
        }

        static void TestTable()
        {
            var test = new TestTable();
            test.Do();
        }
    }
}
