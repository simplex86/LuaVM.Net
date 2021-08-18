using System;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestOperations
    {
        public void Do()
        {
            var state = new LuaState(16);
            state.Push(1);
            state.Push("2.0");
            state.Push("3.0");
            state.Push(4.0);
            PrintState(state);

            state.Arithmetic(0);    //add
            PrintState(state);
            state.Arithmetic(13);   //bit not
            PrintState(state);
            state.Len(2);
            PrintState(state);
            state.Concat(3);
            PrintState(state);

            var b = state.Compare(1, 2, 0); // EQ
            state.Push(b);
            PrintState(state);
        }

        private void PrintState(LuaState state)
        {
            var top = state.GetTop();
            for (int i = 1; i <= top; i++)
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
