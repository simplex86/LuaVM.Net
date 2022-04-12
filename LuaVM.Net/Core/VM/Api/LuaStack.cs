using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class LuaStack
    {
        private List<LuaValue> slots;

        internal LuaStack prev { get; set; }
        internal Closure closure { get; set; }
        internal LuaValue[] varargs { get; set; }
        internal int pc { get; set; } = 0;
        internal LuaState state { get; set; }

        internal LuaStack(int n, LuaState state)
        {
            slots = new List<LuaValue>(n);
            for (int i=0; i<n; i++)
            {
                slots.Add(null);
            }
            top = 0;

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

        internal void PushN(LuaValue[] vals, int n)
        {
            var m = vals.Length;
            if (n < 0)
            {
                n = m;
            }

            for (var i = 0; i < n; i++)
            {
                if (i < m)
                {
                    Push(vals[i]);
                }
                else
                {
                    Push();
                }
            }
        }

        // 压栈
        internal void Push(LuaValue value)
        {
            if (top == capacity)
            {
                Error.Commit("stack overflow!");
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
                Error.Commit("stack overflow!");
            }

            top--;
            var v = slots[top];
            slots[top] = null;

            return v;
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
        internal void Set(int idx, LuaTable value)
        {
            Set(idx, new LuaValue(value));
        }

        // 设置指定索引位置的值
        internal void Set(int idx, LuaValue value)
        {
            if (idx == Consts.LUA_REGISTRYINDEX)
            {
                state.registry = value.GetTable();
                return;
            }

            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                Error.Commit("invalid stack index!");
            }
            slots[idx - 1] = value;
        }

        // 获取指定索引位置的值
        internal LuaValue Get(int idx)
        {
            if (idx == Consts.LUA_REGISTRYINDEX)
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

        // 判断索引是否有效
        internal bool IsValid(int idx)
        {
            if (idx == Consts.LUA_REGISTRYINDEX)
            {
                return true;
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
            if (idx >= 0 || idx < Consts.LUA_REGISTRYINDEX)
            {
                return idx;
            }
            return idx + top + 1;
        }

        // 栈顶索引值
        internal int top { get; set; }

        // 栈的容量
        private int capacity
        {
            get { return slots.Capacity; }
            set { slots.Capacity = value; }
        }
    }
}
