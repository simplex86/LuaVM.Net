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

        public void Push()
        {
            stack.Push(null);
        }

        public void Push(bool b)
        {
            stack.Push(b);
        }

        public void Push(long n)
        {
            stack.Push(n);
        }

        public void Push(double n)
        {
            stack.Push(n);
        }

        public void Push(string s)
        {
            stack.Push(s);
        }

        // 获取类型名字
        public string TypeName(int type)
        {
            switch (type)
            {
                case ValueType.LUA_TNONE:     return "no value";
                case ValueType.LUA_TNIL:      return "nil";
                case ValueType.LUA_TBOOLEAN:  return "boolean";
                case ValueType.LUA_TNUMBER:   return "number";
                case ValueType.LUA_TSTRING:   return "string";
                case ValueType.LUA_TTABLE:    return "table";
                case ValueType.LUA_TFUNCTION: return "function";
                case ValueType.LUA_TTHREAD:   return "thread";
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

            return ValueType.LUA_TNONE;
        }

        // 判断索引位置是否为none
        public bool IsNone(int idx)
        {
            return Type(idx) == ValueType.LUA_TNONE;
        }

        // 判断索引位置是否为nil
        public bool IsNil(int idx)
        {
            return Type(idx) == ValueType.LUA_TNIL;
        }

        // 判断索引位置是否为none或nil
        public bool IsNorneOrNil(int idx)
        {
            return IsNone(idx) || IsNil(idx);
        }

        // 判断索引位置是否为bool类型
        public bool IsBoolean(int idx)
        {
            return Type(idx) == ValueType.LUA_TBOOLEAN;
        }

        // 判断索引位置是否为字符串或数字类型
        public bool IsString(int idx)
        {
            var t = Type(idx);
            return t == ValueType.LUA_TSTRING || t == ValueType.LUA_TNUMBER;
        }

        // 判断索引位置是否为（或可转换为）数字类型
        public bool IsNumber(int idx)
        {
            // TODO 待实现
            return false;
        }

        // 判断索引位置是否为整数类型
        public bool IsInteger(int idx)
        {
            // TODO 待实现
            return false;
        }
    }
}
