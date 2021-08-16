using System;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestValue
    {
        public void Do()
        {
            Console.WriteLine("values:");
            var state = new LuaState();
            state.Push();       PrintState(state);
            state.Push(true);   PrintState(state);
            state.Push(123);    PrintState(state);
            state.Push(456.0);  PrintState(state);
            state.Push("abc");  PrintState(state);
            Console.WriteLine("value done!");
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
