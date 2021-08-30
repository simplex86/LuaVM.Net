using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class LuaStack
    {
        private List<LuaValue> slots;
        private LuaState state;

        // 栈的容量
        private int capacity
        {
            get { return slots.Capacity; }
            set { slots.Capacity = value; }
        }

        // 栈顶索引值
        internal int top { get; set; } = 0;

        // 上一个调用栈
        internal LuaStack prev { get; set; }

        // 闭包
        internal Closure closure { get; set; }

        // 可变参数
        internal LuaValue[] varargs { get; set; }

        // 程序计数
        internal int pc { get; set; } = 0;

        // 
        internal Dictionary<int, Upvalue> openuvs { get; set; } = null;

        internal LuaStack(int size, LuaState state)
        {
            slots = new List<LuaValue>(size);
            for (int i=0; i< size; i++)
            {
                slots.Add(null);
            }
            this.state = state;
        }

        // 检查栈是否可容纳n个数据，如果容纳不了则扩容
        internal void Check(int n)
        {
            var m = capacity - top;
            if (m < n)
            {
                capacity = top + n;
                for (int i = 0; i < n; i++)
                {
                    slots.Add(null);
                }
            }
        }

        // 压栈
        internal void Push()
        {
            Push(new LuaValue());
        }

        // 压栈
        internal void Push(bool value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(long value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(double value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(string value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(Closure value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(LuaTable value)
        {
            Push(new LuaValue(value));
        }

        // 压栈
        internal void Push(LuaValue value)
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            if (value == null)
            {
                value = new LuaValue();
            }

            slots[top] = value;
            top++;
        }

        // 出栈
        internal LuaValue Pop()
        {
            if (top < 1)
            {
                throw new Exception("stack overflow!");
            }

            top--;
            var v = slots[top];
            slots[top] = null;

            return v;
        }

        // 设置指定索引位置的值
        internal void Set(int idx, bool value)
        {
            Set(idx, new LuaValue(value));
        }

        // 设置指定索引位置的值
        internal void Set(int idx, long value)
        {
            Set(idx, new LuaValue(value));
        }

        // 设置指定索引位置的值
        internal void Set(int idx, double value)
        {
            Set(idx, new LuaValue(value));
        }

        // 设置指定索引位置的值
        internal void Set(int idx, string value)
        {
            Set(idx, new LuaValue(value));
        }

        // 设置指定索引位置的值
        internal void Set(int idx, LuaValue value)
        {
            if (idx < StateReg.LUA_REGISTRY_INDEX)
            {
                idx = StateReg.LUA_REGISTRY_INDEX - idx - 1;
                if (closure != null && idx < closure.upvalues.Length)
                {
                    closure.upvalues[idx].value = value;
                    return;
                }

                state.registry = value.GetTable();
                return;
            }

            var absIdx = GetAbsIndex(idx);
            //Console.WriteLine($"{idx} -> {absIdx}");
            if (absIdx <= 0 || absIdx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[absIdx - 1] = value;
        }

        // 获取指定索引位置的值
        internal LuaValue Get(int idx)
        {
            if (idx < StateReg.LUA_REGISTRY_INDEX)
            {
                idx = StateReg.LUA_REGISTRY_INDEX - idx - 1;
                var c = closure;
                if (closure == null || idx >= closure.upvalues.Length)
                {
                    return null;
                }

                return c.upvalues[idx].value;
            }

            if (idx == StateReg.LUA_REGISTRY_INDEX)
            {
                return new LuaValue(state.registry);
            }

            idx = GetAbsIndex(idx);
            if (idx > 0 && idx <= top)
            {
                return slots[idx - 1];
            }

            return null;
        }

        // 获取指定索引位置的值（不转换索引值）
        internal LuaValue Peek(int idx)
        {
            return slots[idx];
        }

        // 设置指定索引位置的table中以k为键的值
        internal void SetTable(int idx, LuaValue k, LuaValue v)
        {
            var s = Get(idx);
            if (s.IsTable())
            {
                var t = s.GetTable();
                t.Set(k, v);
                return;
            }

            Error.Commit($"LuaStack SetTable: not a table! idx = {idx}");
        }

        // 判断索引是否有效
        internal bool IsValid(int idx)
        {
            if (idx < StateReg.LUA_REGISTRY_INDEX)
            {
                idx = StateReg.LUA_REGISTRY_INDEX - idx - 1;
                return closure != null && idx < closure.upvalues.Length;
            }

            idx = GetAbsIndex(idx);
            return idx > 0 && idx <= top;
        }

        // 反向
        internal void Reverse(int from, int to)
        {
            if (from != to)
            {
                int index = Math.Min(from, to);
                int count = Math.Abs(from - to) + 1;
                slots.Reverse(index, count);
            }
        }

        // 获取（正数）索引值
        internal int GetAbsIndex(int idx)
        {
            if (idx >= 0 || idx < StateReg.LUA_REGISTRY_INDEX)
            {
                return idx;
            }
            return (idx > 0) ? idx : idx + top + 1;
        }

        internal void PushN(LuaValue[] vals, int n)
        {
            var len = vals.Length;
            if (n < 0)
            {
                n = len;
            }

            for (var i = 0; i < n; i++)
            {
                if (i < len)
                {
                    Push(vals[i]);
                }
                else
                {
                    Push();
                }
            }
        }

        internal LuaValue[] PopN(int n)
        {
            var vals = new LuaValue[n];
            for (var i = n - 1; i >= 0; i--)
            {
                vals[i] = Pop();
            }

            return vals;
        }

        internal void Print(string prefix)
        {
            System.Console.Write($"{prefix}: ");
            foreach (var v in slots)
            {
                if (v == null)
                {
                    System.Console.Write("[nil]");
                }
                else if (v.IsNil())
                {
                    System.Console.Write("[nil]");
                }
                else if (v.IsBoolean())
                {
                    System.Console.Write($"[{v.GetBoolean()}]");
                }
                else if (v.IsInteger())
                {
                    System.Console.Write($"[{v.GetInteger()}]");
                }
                else if (v.IsFloat())
                {
                    System.Console.Write($"[{v.GetFloat()}]");
                }
                else if (v.IsString())
                {
                    System.Console.Write($"[{v.GetString()}]");
                }
                else if (v.IsFunction())
                {
                    System.Console.Write("[function]");
                }
                else if (v.IsTable())
                {
                    System.Console.Write("[table]");
                }
            }
            System.Console.WriteLine("");
        }
    }
}
