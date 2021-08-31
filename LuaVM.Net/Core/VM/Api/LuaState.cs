using System;
using System.Collections.Generic;
using System.Linq;

namespace LuaVM.Net.Core
{
    public class LuaState
    {
        // 栈
        private LuaStack stack;

        private Prototype proto
        {
            get { return stack.closure.proto; }
        }

        // 注册表
        internal LuaTable registry { get; set; }

        // 获取程序计数
        public int pc 
        {
            get { return stack.pc; } 
            private set { stack.pc = value; }
        }

        public LuaState()
        {
            stack = new LuaStack(StackSize.LUA_STACK_MIN, this);

            registry = new LuaTable();
            registry.Set(StateReg.LUA_REGISTRY_GLOBALS, new LuaValue(new LuaTable()));

            var nstack = new LuaStack(StackSize.LUA_STACK_MIN, this);
            PushStack(nstack);

            Register("print", Methods.Print);
        }

        // 加载prototype
        public int Load(Prototype proto)
        {
            var c = new Closure(proto);
            stack.Push(c);

            if (proto.upvalues.Length > 0)
            {
                var env = registry.Get(StateReg.LUA_REGISTRY_GLOBALS);
                c.upvalues[0] = new Upvalue(env);
            }

            return 0;
        }

        // 调用函数
        public void Call(int args, int results)
        {
            try
            {
                var v = stack.Get(-(args + 1));
                if (!v.IsFunction())
                {
                    Error.Commit("\ntry to call a non-function type!");
                    return;
                }

                var c = v.GetFunction();
                if (c.proto != null)
                {
                    //Console.WriteLine($"\ncall lua function {c.proto.source}<{c.proto.lineDefined},{c.proto.lastLineDefined}>");
                    CallClosure(args, results, c);
                }
                else
                {
                    //Console.WriteLine("\ncall csharp function>");
                    CallFunction(args, results, c);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\nerror: {ex.Message}\nstacktrace:\n{ex.StackTrace}");
            }
        }

        // 调用Lua闭包
        private void CallClosure(int args, int results, Closure c)
        {
            var nRegs = (int)c.proto.maxStackSize;
            var nParams = (int)c.proto.numParams;
            var isVararg = c.proto.isVararg == 1;

            // 创建新的栈
            var newStack = new LuaStack(nRegs + StackSize.LUA_STACK_MIN, this);
            newStack.closure = c;

            // 弹出函数，并把参数压如新的栈
            var funcAndArgs = stack.PopN(args + 1);
            newStack.PushN(funcAndArgs.Skip(1).ToArray(), nParams);
            newStack.top = nRegs;
            if (args > nParams && isVararg)
            {
                newStack.varargs = funcAndArgs.Skip(nParams + 1).ToArray();
            }

            // 执行闭包
            PushStack(newStack); 
            RunClosure(); 
            PopStack(); 

            // 返回值
            if (results != 0)
            {
                var rs = newStack.PopN(newStack.top - nRegs);
                stack.Check(rs.Length);
                stack.PushN(rs, results);
            }
        }

        // 调用CSharp函数
        private void CallFunction(int args, int results, Closure c)
        {
            var newStack = new LuaStack(args + StackSize.LUA_STACK_MIN, this);
            newStack.closure = c;

            var funcAndArgs = stack.PopN(args);
            newStack.PushN(funcAndArgs, args);
            stack.Pop();

            // 调用函数
            PushStack(newStack);
            var rn = RunFunction(c);
            PopStack();

            // 返回值
            if (results != 0)
            {
                var rs = newStack.PopN(rn);
                stack.Check(rs.Length);
                stack.PushN(rs, results);
            }
        }

        // 执行闭包
        private void RunClosure()
        {
            while (true)
            {
                var inst = new Instruction(Fetch());
                inst.Execute(this);

                if (inst.OpCode() == OperationCodes.OP_RETURN)
                {
                    break;
                }
            }
        }

        // 调用CSharp函数
        private int RunFunction(Closure c)
        {
            return c.cfunc.Invoke(this);
        }

        // 从指定位置加载prototype
        public void LoadProto(int idx)
        {
            var proto = this.proto.protos[idx];
            var c = new Closure(proto);
            stack.Push(c);

            for (int i=0; i< proto.upvalues.Length; i++)
            {
                var uv = proto.upvalues[i];
                if (uv.instack == 1)
                {
                    if (stack.openuvs == null)
                    {
                        stack.openuvs = new Dictionary<int, Upvalue>();
                    }

                    if (stack.openuvs.ContainsKey(uv.idx))
                    {
                        c.upvalues[i] = stack.openuvs[uv.idx];
                    }
                    else
                    {
                        c.upvalues[i] = new Upvalue(stack.Peek(uv.idx));
                        stack.openuvs[uv.idx] = c.upvalues[i];
                    }
                }
                else
                {
                    c.upvalues[i] = stack.closure.upvalues[uv.idx];
                }
            }
        }

        // 加载可变参数
        public void LoadVararg(int n)
        {
            if (n < 0)
            {
                n = stack.varargs.Length;
            }

            stack.Check(n);
            stack.PushN(stack.varargs, n);
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

        // 获取寄存器数量
        public int RegisterCount()
        {
            return proto.maxStackSize;
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

        // 压入CSharp函数
        public void Push(CSharpFunction func)
        {
            var c = new Closure(func);
            stack.Push(c);
        }

        // 把指定位置的压入栈顶
        public void PushV(int idx)
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
            return (value == null) ? false : value.IsFloat();
        }

        // 判断索引位置是否为整数类型
        public bool IsInteger(int idx)
        {
            var value = stack.Get(idx);
            return (value == null) ? false : value.IsInteger();
        }

        // 判断索引位置是否为整数类型
        internal bool IsCSharpFunction(int idx)
        {
            var value = stack.Get(idx);
            if (value == null || !value.IsFunction())
            {
                return false;
            }

            var c = value.GetFunction();
            return c.cfunc != null;
        }

        // 把指定位置转换成布尔值(nil和false,其他为true)
        public bool ToBoolean(int idx)
        {
            var value = stack.Get(idx);
            return ToBoolean(value);
        }

        private bool ToBoolean(LuaValue value)
        {
            if (value.IsNil())
            {
                return false;
            }
            if (value.IsBoolean())
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
            if (value.IsString())
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
                return (long)value.GetFloat();
            }
            if (value.IsInteger())
            {
                return value.GetInteger();
            }
            if (value.IsString())
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
            if (value.IsString())
            {
                return value.GetString();
            }
            if (value.IsFloat())
            {
                var n = value.GetFloat();
                return n.ToString();
            }
            if (value.IsInteger())
            {
                var n = value.GetInteger();
                return n.ToString();
            }

            return string.Empty;
        }

        // 把指定位置转换成CSharp函数
        public CSharpFunction ToCSharpFunction(int idx)
        {
            var v = stack.Get(idx);
            if (LuaValue.GetType(v) != LuaType.LUA_TFUNCTION)
            {
                return null;
            }

            var c = v.GetFunction();
            return c.cfunc;
        }

        // 比较指定位置上的两个值
        public bool Compare(int idx1, int idx2, int op)
        {
            if (!stack.IsValid(idx1) || !stack.IsValid(idx2))
            {
                return false;
            }
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
        public void GetConst(int idx)
        {
            var c = proto.constants[idx];
            stack.Push(c);
        }

        // 将指定位置的常量或栈值压到栈顶
        public void GetRK(int rk)
        {
            if (rk > 0xFF)
            {
                GetConst(rk & 0xFF);
            }
            else
            {
                PushV(rk + 1);
            }
        }

        // 创建table
        public void CreateTable()
        {
            CreateTable(0, 0);
        }

        // 创建table
        public void CreateTable(int n, int m)
        {
            var t = new LuaTable(n, m);
            stack.Push(t);
        }

        // 从指定位置获取table
        public int GetTable(int idx)
        {
            var t = stack.Get(idx);
            var k = stack.Pop();
            return GetTable(t, k);
        }

        // 把table设置到指定位置
        public void SetTable(int idx)
        {
            var v = stack.Pop();
            var k = stack.Pop();
            SetTable(idx, k, v);
        }

        // 获取 idx 位置的table中 i 索引的值
        public int GetI(int idx, long i)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(i);
            return GetTable(t, k);
        }

        // 设置 idx 位置的table中 i 索引的值
        public void SetI(int idx, long n)
        {
            var k = new LuaValue(n);
            var v = stack.Pop();
            SetTable(idx, k, v);
        }

        // 获取 idx 位置的table中 k 键值的值
        public int GetField(int idx, string s)
        {
            var t = stack.Get(idx);
            var k = new LuaValue(s);
            return GetTable(t, k);
        }

        // 设置 idx 位置的table中 k 键值的值
        public void SetField(int idx, string s)
        {
            var k = new LuaValue(s);
            var v = stack.Pop();
            SetTable(idx, k, v);
        }

        private int GetTable(LuaValue t, LuaValue k)
        {
            if (LuaValue.GetType(t) != LuaType.LUA_TTABLE)
            {
                Error.Commit("LuaState GetTable: not a table!");
                return LuaType.LUA_TNONE;
            }

            var v = t.GetTable().Get(k);
            stack.Push(v);

            return v.type;
        }

        private void SetTable(int idx, LuaValue k, LuaValue v)
        {
            var t = stack.Get(idx);
            SetTable(t, k, v);
        }

        private void SetTable(LuaValue t, LuaValue k, LuaValue v)
        {
            if (LuaValue.GetType(t) == LuaType.LUA_TTABLE)
            {
                t.GetTable().Set(k, v);
                return;
            }

            Error.Commit("LuaState SetTable: not a table!");
        }

        // 压入调用栈
        internal void PushStack(LuaStack stack)
        {
            stack.prev = this.stack;
            this.stack = stack;
        }

        // 弹出调用栈
        internal void PopStack()
        {
            var s = stack;
            stack = s.prev;
            s.prev = null;
        }

        // 
        public void PushGlobalTable()
        {
            var g = registry.Get(StateReg.LUA_REGISTRY_GLOBALS);
            stack.Push(g);
        }

        // 
        public int GetGlobal(string name)
        {
            var t = registry.Get(StateReg.LUA_REGISTRY_GLOBALS);
            return GetTable(t, new LuaValue(name));
        }

        // 
        public void SetGlobal(string name)
        {
            var t = registry.Get(StateReg.LUA_REGISTRY_GLOBALS);
            var v = stack.Pop();
            SetTable(t, new LuaValue(name), v);
            registry.Set(StateReg.LUA_REGISTRY_GLOBALS, t);
        }

        public void Register(string name, CSharpFunction func)
        {
            Push(func);
            SetGlobal(name);
        }

        // 闭合Upvalue
        internal void CloseUpvalues(int n)
        {
            List<int> keys = new List<int>();

            foreach (var kvp in stack.openuvs)
            {
                var k = kvp.Key;
                var v = kvp.Value;

                if (k >= n - 1)
                {
                    var val = v.value;
                    v.value = val;
                    keys.Add(k);
                }
            }

            foreach (var k in keys)
            {
                stack.openuvs.Remove(k);
            }
        }

        internal void PrintStack(string prefix)
        {
            stack.Print(prefix);
        }
    }
}
