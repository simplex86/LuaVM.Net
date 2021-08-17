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
            
            state.Push(true);   PrintState(state);
            state.Push(123);    PrintState(state);
            state.Push();       PrintState(state);
            state.Push("abc");  PrintState(state);
            state.Push(-456);   PrintState(state);
            state.Replace(3);   PrintState(state);
            state.SetTop(6);    PrintState(state);
            state.Remove(-3);   PrintState(state);
            state.SetTop(-5);   PrintState(state);
            Console.WriteLine("value test done!");
        }

        private void PrintState(LuaState state)
        {
            var top = state.GetTop();
            for (int i=1; i<=top; i++)
            {
                var type = state.Type(i);
                switch (type)
                {
                    case LuaType.LUA_TBOOLEAN:
                        Console.WriteLine($"[{i}]:{state.ToBoolean(i)}");
                        break;
                    case LuaType.LUA_TNUMBER:
                        Console.WriteLine($"[{i}]:{state.ToNumber(i)}");
                        break;
                    case LuaType.LUA_TSTRING:
                        Console.WriteLine($"[{i}]:{state.ToString(i)}");
                        break;
                    default:
                        Console.WriteLine($"[{i}]:{state.TypeName(type)}");
                        break;
                }
            }
            Console.WriteLine();
        }
    }
}
