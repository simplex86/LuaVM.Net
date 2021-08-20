using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    public class LuaState
    {
        private LuaStack  stack;
        private Prototype proto;

        public int pc { get; private set; } = 0;

        public LuaState()
        {
            this.stack = new LuaStack(32);
        }

        public LuaState(int size)
        {
            this.stack = new LuaStack(size);
        }

        public LuaState(int size, Prototype proto)
        {
            this.stack = new LuaStack(size);
            this.proto = proto;
        }

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

        // 弹出栈顶
        public void Pop(int n)
        {
            SetTop(-n - 1);
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

        // 把指定位置的压入栈顶
        public void PushX(int idx)
        {
            var v = stack.Get(idx);
            stack.Push(v);
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
                return LuaValue.GetType(v);
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

        // 把指定位置转换成布尔值(nil和false,其他为true)
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

        // 把指定位置转换成小数(包括整数和可以转换的字符串)
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

        // 把指定位置转换成整数(包括小数和可以转换的字符串)
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

        // 把指定位置转换成字符串(包括整数和小数)
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

        // 比较指定位置上的两个值
        public bool Compare(int idx1, int idx2, int op)
        {
            var a = stack.Get(idx1);
            var b = stack.Get(idx2);
            return Core.Compare.Do(a, b, op);
        }

        // 计算
        public void Arithmetic(int op)
        {
            var b = stack.Pop();
            var a = b;

            if (op != Operations.LUA_OPUNM && op != Operations.LUA_OPBNOT)
            {
                a = stack.Pop();
            }

            var r = Core.Arithmetic.Do(a, b, op);
            if (r == null)
            {
                Error.Commit("arithmetic error!");
                return;
            }
            stack.Push(r);
        }

        // 长度
        public void Len(int idx)
        {
            var v = stack.Get(idx);
            if (v.IsString())
            {
                var n = String.Len(v);
                stack.Push(n);
            }
            else if (v.IsTable())
            {
                var n = Table.Len(v);
                stack.Push(n);
            }
            else
            {
                Error.Commit("length error!");
            }
        }

        // 连接
        public void Concat(int n)
        {
            if (n == 0)
            {
                stack.Push();
                return;
            }

            if (n >= 2)
            {
                for (var i = 1; i < n; i++)
                {
                    if (IsString(-1) && IsString(-2))
                    {
                        var b = stack.Get(-1);
                        var a = stack.Get(-2);

                        stack.Pop();
                        stack.Pop();

                        var s = String.Concat(a, b);
                        stack.Push(s);

                        continue;
                    }
                    Error.Commit("string concatenation error!");
                }
            }
        }

        // 增加PC
        public void AddPC(int n)
        {
            pc += n;
        }

        // 取出当前指令，并将PC指向下一条指令
        public uint Fetch()
        {
            var inst = proto.code[pc];
            pc++;
            return inst;
        }

        // 将指定位置的常量压到栈顶
        public void PushConst(int idx)
        {
            var c = proto.constants[idx];
            switch (c.GetType().Name)
            {
                case "Int64":
                    stack.Push((long)c);
                    break;
                case "Double":
                    stack.Push((double)c);
                    break;
                case "String":
                    stack.Push(c as string);
                    break;
            }
        }

        // 将指定位置的常量或栈值压到栈顶
        public void PushRK(int rk)
        {
            if (rk > 0xFF)
            {
                PushConst(rk & 0xFF);
            }
            else
            {
                PushX(rk + 1);
            }
        }

        // 
        public void CreateTable()
        {
            CreateTable(0, 0);
        }

        // 
        public void CreateTable(int n, int m)
        {
            var t = new LuaTable(n, m);
            stack.Push(t);
        }

        // 
        public int GetTable(int idx)
        {
            var t = stack.Get(idx);
            var k = stack.Pop();
            return GetTable(t, k);
        }

        //
        public void SetTable(int idx)
        {
            var t = stack.Get(idx);
            var v = stack.Pop();
            var k = stack.Pop();
            SetTable(t, k, v);
        }

        // 
        public int GetI(int idx, long i)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(i);
            return GetTable(t, k);
        }

        // 
        public void SetI(int idx, long n)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(n);
            var v = stack.Pop();
            SetTable(t, k, v);
        }

        // 
        public int GetField(int idx, string s)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(s);
            return GetTable(t, k);
        }

        // 
        public void SetField(int idx, string s)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(s);
            var v = stack.Pop();
            SetTable(t, k, v);
        }

        private int GetTable(LuaValue t, LuaValue k)
        {
            if (!t.IsTable())
            {
                Error.Commit("not a table!");
                return LuaType.LUA_TNONE;
            }

            var v = t.GetTable().Get(k);
            stack.Push(v);

            return v.type;
        }

        private void SetTable(LuaValue t, LuaValue k, LuaValue v)
        {
            if (t.IsTable())
            {
                t.GetTable().Set(k, v);
                return;
            }

            Error.Commit("not a table!");
        }
    }
}
