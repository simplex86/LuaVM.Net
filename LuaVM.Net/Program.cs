﻿using System;
using System.IO;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMetatable();
            // pause
            Console.Write("\npress any key to be continue...");
            Console.ReadKey();
        }

        // 执行词法分析器的测试
        static void TestLexer()
        {
            var test = new TestLexer();
            test.Do(@"Luas\lexer.lua");
        }

        // 执行语法分析器的测试
        static void TestParser()
        {
            var test = new TestParser();
            test.Do(@"Luas\lexer.lua");
        }

        // 执行字节反编译器的测试
        static void TestUndump()
        {
            var test = new TestUndump();
            test.Do(@"Luas\vm.out");
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
            test.Do(@"Luas\vm.out");
        }

        static void TestTable()
        {
            var test = new TestTable();
            test.Do(@"Luas\table.out");
        }

        static void TestFunction()
        {
            var test = new TestFunction();
            test.Do(@"Luas\function.out");
        }

        static void TestClosure()
        {
            var test = new TestClosure();
            test.Do(@"Luas\closure.out");
        }

        static void TestMetatable()
        {
            var test = new TestMetatable();
            test.Do(@"Luas\metatable.out");
        }
    }
}
