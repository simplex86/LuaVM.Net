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
        }

        // 检查栈是否可容纳n个数据，如果容纳不了则扩容
        internal void Check(int n)
        {
            var m = capacity - top;
            if (m < n)
            {
                capacity += m;
            }
        }

        // 压栈
        internal void Push()
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            slots.Add(new LuaValue());
        }

        // 压栈
        internal void Push(bool value)
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            slots.Add(new LuaValue(value));
        }

        // 压栈
        internal void Push(long value)
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            slots.Add(new LuaValue(value));
        }

        // 压栈
        internal void Push(double value)
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            slots.Add(new LuaValue(value));
        }

        // 压栈
        internal void Push(string value)
        {
            if (top == capacity)
            {
                throw new Exception("stack overflow!");
            }

            slots.Add(new LuaValue(value));
        }

        // 出栈
        internal LuaValue Pop()
        {
            if (top == 0)
            {
                throw new Exception("stack overflow!");
            }

            var v = slots[top - 1];
            slots.RemoveAt(top - 1);

            return v;
        }

        // 设置指定索引位置的值
        internal void Set(int idx, bool value)
        {
            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[idx - 1] = new LuaValue(value);
        }

        // 设置指定索引位置的值
        internal void Set(int idx, long value)
        {
            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[idx - 1] = new LuaValue(value);
        }

        // 设置指定索引位置的值
        internal void Set(int idx, double value)
        {
            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[idx - 1] = new LuaValue(value);
        }

        // 设置指定索引位置的值
        internal void Set(int idx, string value)
        {
            idx = GetAbsIndex(idx);
            if (idx <= 0 || idx > top)
            {
                throw new Exception("invalid stack index!");
            }
            slots[idx - 1] = new LuaValue(value);
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
            return (idx > 0) ? idx : idx + top;
        }

        // 栈顶索引值
        internal int top
        {
            get { return slots.Count; }
        }

        // 栈的容量
        private int capacity
        {
            get { return slots.Capacity; }
            set { slots.Capacity = value; }
        }
    }
}
