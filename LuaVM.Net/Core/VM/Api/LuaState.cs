using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    public class LuaState
    {
        private LuaStack stack = new LuaStack(20);

        // 获取栈顶索引值
        public int GetTop()
        {
            return stack.top;
        }

        // 设置栈顶
        public void SetTop(int idx)
        {
            var top = stack.GetAbsIndex(idx);
            if (top < 0)
            {
                Error.Commit("stack underflow");
                return;
            }

            var n = stack.top - top;
            if (n > 0)
            {
                for (int i=0; i<n; i++)
                {
                    stack.Pop();
                }
            }
            else if (n < 0)
            {
                for (int i = 0; i > n; i--)
                {
                    stack.Push();
                }
            }
        }

        // 转换为绝对索引
        public int AbsIndex(int index)
        {
            return stack.GetAbsIndex(index);
        }

        public bool Check(int n)
        {
            stack.Check(n);
            return true;
        }

        public void Pop(int n)
        {
            for (int i=0; i<n; i++)
            {
                stack.Pop();
            }
        }

        // 压入nil
        public void Push()
        {
            stack.Push();
        }

        // 压入布尔值
        public void Push(bool b)
        {
            stack.Push(b);
        }

        // 压入整数
        public void Push(long n)
        {
            stack.Push(n);
        }

        // 压入小数（浮点数）
        public void Push(double n)
        {
            stack.Push(n);
        }

        // 压入字符串
        public void Push(string s)
        {
            stack.Push(s);
        }

        // 将栈顶弹出，然后插入到指定位置
        public void Insert(int idx)
        {
            Rotate(idx, 1);
        }

        // 从from位置复制到to位置
        public void Copy(int from, int to)
        {
            var value = stack.Get(from);
            stack.Set(to, value);
        }

        //
        public void Rotate(int idx, int n)
        {
            var t = stack.top - 1;
            var p = stack.GetAbsIndex(idx) - 1;

            var m = (n >= 0) ? (t - n) : (p - n - 1);
            stack.Reverse(p, m);
            stack.Reverse(m + 1, t);
            stack.Reverse(p, t);
        }

        // 删除idx位置的值，并将该位置上面的值全部下移
        public void Remove(int idx)
        {
            Rotate(idx, -1);
            Pop(1);
        }

        // 将栈顶弹出，再插入到指定位置
        public void Replace(int idx)
        {
            var value = stack.Pop();
            stack.Set(idx, value);
        }

        // 获取类型名字
        public string TypeName(int type)
        {
            switch (type)
            {
                case LuaType.LUA_TNONE:     return "no value";
                case LuaType.LUA_TNIL:      return "nil";
                case LuaType.LUA_TBOOLEAN:  return "boolean";
                case LuaType.LUA_TNUMBER:   return "number";
                case LuaType.LUA_TSTRING:   return "string";
                case LuaType.LUA_TTABLE:    return "table";
                case LuaType.LUA_TFUNCTION: return "function";
                case LuaType.LUA_TTHREAD:   return "thread";
                default: return "userdata";
            }
        }

        // 根据索引获取类型
        public int Type(int idx)
        {
            if (stack.IsValid(idx))
            {
                var v = stack.Get(idx);
                return v.type;
            }

            return LuaType.LUA_TNONE;
        }

        // 判断索引位置是否为none
        public bool IsNone(int idx)
        {
            return Type(idx) == LuaType.LUA_TNONE;
        }

        // 判断索引位置是否为nil
        public bool IsNil(int idx)
        {
            return Type(idx) == LuaType.LUA_TNIL;
        }

        // 判断索引位置是否为none或nil
        public bool IsNorneOrNil(int idx)
        {
            return IsNone(idx) || IsNil(idx);
        }

        // 判断索引位置是否为bool类型
        public bool IsBoolean(int idx)
        {
            return Type(idx) == LuaType.LUA_TBOOLEAN;
        }

        // 判断索引位置是否为字符串或数字类型
        public bool IsString(int idx)
        {
            var t = Type(idx);
            return t == LuaType.LUA_TSTRING || t == LuaType.LUA_TNUMBER;
        }

        // 判断索引位置是否为（或可转换为）数字类型
        public bool IsNumber(int idx)
        {
            var value = stack.Get(idx);
            return value.IsFloat();
        }

        // 判断索引位置是否为整数类型
        public bool IsInteger(int idx)
        {
            var value = stack.Get(idx);
            return value.IsInteger();
        }

        public bool ToBoolean(int idx)
        {
            var value = stack.Get(idx);
            return ToBoolean(value);
        }

        private bool ToBoolean(LuaValue value)
        {
            if (value.type == LuaType.LUA_TNIL)
            {
                return false;
            }
            if (value.type == LuaType.LUA_TBOOLEAN)
            {
                return value.GetBoolean();
            }

            return true;
        }

        public double ToNumber(int idx)
        {
            var value = stack.Get(idx);
            return ToNumber(value);
        }

        private double ToNumber(LuaValue value)
        {
            if (value.IsFloat())
            {
                return value.GetFloat();
            }
            if (value.IsInteger())
            {
                var n = value.GetInteger();
                return (double)n;
            }
            if (value.type == LuaType.LUA_TSTRING)
            {
                var s = value.GetString();
                double n = 0.0;
                if (double.TryParse(s, out n))
                {
                    return n;
                }
            }

            return 0.0;
        }

        public long ToInteger(int idx)
        {
            var value = stack.Get(idx);
            return ToInteger(value);
        }

        private long ToInteger(LuaValue value)
        {
            if (value.IsFloat())
            {
                return value.GetInteger();
            }
            if (value.IsInteger())
            {
                var n = value.GetFloat();
                return (int)n;
            }
            if (value.type == LuaType.LUA_TSTRING)
            {
                var s = value.GetString();
                long n = 0;
                if (long.TryParse(s, out n))
                {
                    return n;
                }
            }

            return 0;
        }

        public string ToString(int idx)
        {
            var value = stack.Get(idx);
            return ToString(value);
        }

        private string ToString(LuaValue value)
        {
            if (value.type == LuaType.LUA_TSTRING)
            {
                return value.GetString();
            }
            if (value.IsFloat())
            {
                var n = value.GetInteger();
                return n.ToString();
            }
            if (value.IsInteger())
            {
                var n = value.GetFloat();
                return n.ToString();
            }

            return string.Empty;
        }
    }
}
