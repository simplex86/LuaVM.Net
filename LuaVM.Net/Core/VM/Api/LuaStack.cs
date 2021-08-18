using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal struct LuaStack
    {
        private List<LuaValue> slots;

        internal LuaStack(int n)
        {
            slots = new List<LuaValue>(n);
            for (int i=0; i<n; i++)
            {
                slots.Add(null);
            }
            top = 0;
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
            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[idx - 1] = value;
        }

        // 获取指定索引位置的值
        internal LuaValue Get(int idx)
        {
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
            return (idx > 0) ? idx : idx + top + 1;
        }

        // 栈顶索引值
        internal int top { get; private set; }

        // 栈的容量
        private int capacity
        {
            get { return slots.Capacity; }
            set { slots.Capacity = value; }
        }
    }
}
