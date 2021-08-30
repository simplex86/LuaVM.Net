using System;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestValue
    {
        public void Do()
        {
            Console.WriteLine("value test:");
            var state = new LuaState();
            
            state.Push(true);   Test.PrintState(state);
            state.Push(123);    Test.PrintState(state);
            state.Push();       Test.PrintState(state);
            state.Push("abc");  Test.PrintState(state);
            state.Push(-456);   Test.PrintState(state);
            state.Replace(3);   Test.PrintState(state);
            state.SetTop(6);    Test.PrintState(state);
            state.Remove(-3);   Test.PrintState(state);
            state.SetTop(-5);   Test.PrintState(state);
            Console.WriteLine("value test done!");
        }
    }
}
