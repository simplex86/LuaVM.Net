using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal static class VM
    {
        // 在寄存器间拷贝值
        // MOVE
        // A B      R(A) := R(B)
        internal static void Move(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.Copy(b, a);
        }

        // 无条件跳转
        // JMP
        // sBx      pc += sBx
        internal static void Jump(Instruction i, LuaState ls)
        {
            var t = i.AsBx();
            var a = t.Item1;
            var b = t.Item2;
            System.Console.Write($"[AsBx:{a}|{b}]");
            ls.AddPC(b);
            if (a != 0)
            {
                ls.CloseUpvalues(a);
            }
        }

        // 加载nil给寄存器
        // LOADNIL
        // A B      R(A), R(A+1), ..., R(A+B) := nil
        internal static void LoadNil(Instruction i, LuaState ls)
        {
            var t = i.AsBx();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[AsBx:{a}|{b}]");
            ls.Push();

            for (int k = a; k<= a+b; k++)
            {
                ls.Copy(-1, k);
            }
            ls.Pop(1);
        }

        // 设置布尔值B给R(A)，如果C为true则跳过下一条指令
        // LOADBOOL
        // A B C    R(A) := (Bool)B; if(C) pc++
        internal static void LoadBoolean(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.Push(b != 0);
            ls.Replace(a);
            if (c != 0)
            {
                ls.AddPC(1);
            }
        }

        // 加载常量给寄存器
        // LOADK
        // A Bx     R(A) := Kst(Bx)
        internal static void LoadK(Instruction i, LuaState ls)
        {
            var t = i.ABx();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[ABx:{a}|{b}]");
            ls.GetConst(b);
            ls.Replace(a);
        }

        // 加载常量到寄存器，常量索引从下一条指令OP_EXTRAARG获取，也就是下一行指令必定是OP_EXTRAARG
        // LOADKX
        // A        R(A) := Kst(extra arg)
        internal static void LoadKx(Instruction i, LuaState ls)
        {
            var t = i.ABx();
            var a = t.Item1 + 1;
            var p = new Instruction(ls.Fetch());
            var b = p.Ax();
            System.Console.Write($"[ABx:{a}|{b}]");
            ls.GetConst(b);
            ls.Replace(a);
        }

        // 一元运算
        private static void UnaryOp(Instruction i, LuaState ls, int op)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.PushV(b);
            ls.Arithmetic(op);
            ls.Replace(a);
        }

        // 二元运算
        private static void BinaryOp(Instruction i, LuaState ls, int op)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(b);
            ls.GetRK(c);
            ls.Arithmetic(op);
            ls.Replace(a);
        }

        // ADD
        // A B C    R(A) := RK(B) + RK(C)
        internal static void Add(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPADD);
        }

        // SUB
        // A B C    R(A) := RK(B) - RK(C)
        internal static void Sub(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSUB);
        }

        // MUL
        // A B C	R(A) := RK(B) * RK(C)
        internal static void Mul(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPMUL);
        }

        // MOD
        // A B C	R(A) := RK(B) % RK(C)
        internal static void Mod(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPMOD);
        }

        // POW
        // A B C	R(A) := RK(B) ^ RK(C)
        internal static void Pow(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPPOW);
        }

        // DIV
        // A B C	R(A) := RK(B) / RK(C)
        internal static void Div(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPDIV);
        }

        // IDIV
        // A B C	R(A) := RK(B) // RK(C)
        internal static void IDiv(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPIDIV);
        }

        // BAND
        // A B C	R(A) := RK(B) & RK(C)
        internal static void BAnd(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBAND);
        }

        // BOR
        // A B C	R(A) := RK(B) | RK(C)
        internal static void BOr(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBOR);
        }

        // BXOR
        // A B C	R(A) := RK(B) ~ RK(C)
        internal static void BXor(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBXOR);
        }

        // SHL
        // A B C	R(A) := RK(B) << RK(C)
        internal static void Shl(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSHL);
        }

        // SHR
        // A B C	R(A) := RK(B) >> RK(C)
        internal static void Shr(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSHR);
        }

        // UNM
        // A B	    R(A) := -R(B)
        internal static void Unm(Instruction i, LuaState ls)
        {
            UnaryOp(i, ls, Operations.LUA_OPUNM);
        }

        // BNOT
        // A B	    R(A) := ~R(B)
        internal static void BNot(Instruction i, LuaState ls)
        {
            UnaryOp(i, ls, Operations.LUA_OPBNOT);
        }

        // LEN
        // A B	    R(A) := length of R(B)
        internal static void Len(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.Len(b);
            ls.Replace(a);
        }

        // CONCAT
        // A B C	R(A) := R(B).. ... ..R(C)
        internal static void Concat(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3 + 1;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            var n = c - b + 1;
            ls.Check(n);

            for (var k = b; k <= c; k++)
            {
                ls.PushV(k);
            }

            ls.Concat(n);
            ls.Replace(a);
        }

        // 比较
        private static void Compare(Instruction i, LuaState ls, int op)
        {
            var t = i.ABC();
            var a = t.Item1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(b);
            ls.GetRK(c);
            if (ls.Compare(-2, -1, op) != (a != 0))
            {
                ls.AddPC(1);
            }

            ls.Pop(2);
        }

        // EQ
        // A B C	if ((RK(B) == RK(C)) ~= A) then pc++
        internal static void EQ(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPEQ);
        }

        // LT
        // A B C	if ((RK(B) <  RK(C)) ~= A) then pc++
        internal static void LT(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPLT);
        }

        // LE
        // A B C	if ((RK(B) <= RK(C)) ~= A) then pc++
        internal static void LE(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPLE);
        }

        // NOT
        // A B	    R(A) := not R(B)
        internal static void Not(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.Push(!ls.ToBoolean(b));
            ls.Replace(a);
        }

        // TEST
        // A C	    if not (R(A) <=> C) then pc++
        internal static void Test(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|N|{c}]");
            if (ls.ToBoolean(a) != (c != 0))
            {
                ls.AddPC(1);
            }
        }

        // TESTSET
        // A B C	if (R(B) <=> C) then R(A) := R(B) else pc++
        internal static void TestSet(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            if (ls.ToBoolean(b) == (c != 0))
            {
                ls.Copy(b, a);
            }
            else
            {
                ls.AddPC(1);
            }
        }

        // FORPREP
        // A sBx	R(A) -= R(A+2); pc += sBx
        internal static void ForPrep(Instruction i, LuaState ls)
        {
            var t = i.AsBx();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[AsBx:{a}|{b}]");
            ls.PushV(a);
            ls.PushV(a + 2);
            ls.Arithmetic(Operations.LUA_OPSUB);
            ls.Replace(a);
            ls.AddPC(b);
        }

        // FORLOOP
        // A sBx	if R(A+1) ~= nil then { R(A) = R(A+1); pc += sBx }
        internal static void ForLoop(Instruction i, LuaState ls)
        {
            var t = i.AsBx();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[AsBx:{a}|{b}]");
            // R(A)+=R(A+2);
            ls.PushV(a + 2);
            ls.PushV(a);
            ls.Arithmetic(Operations.LUA_OPADD);
            ls.Replace(a);

            var isPositiveStep = ls.ToNumber(a + 2) >= 0;
            if ((isPositiveStep  && ls.Compare(a, a + 1, Operations.LUA_OPLE)) ||
                (!isPositiveStep && ls.Compare(a + 1, a, Operations.LUA_OPLE)))
            {
                // pc+=sBx; R(A+3)=R(A)
                ls.AddPC(b);
                ls.Copy(a, a + 3);
            }
        }

        // 新建一个表，表的数组和哈希初始大小分别是B，C
        // NEWTABLE
        // A B C    R(A) := {} (size = B,C)
        internal static void NewTable(Instruction i, LuaState ls)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2;
            var c = abc.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.CreateTable(Fb2Int(b), Fb2Int(c));
            ls.Replace(a);
        }

        // 从第B个寄存器取出表，然后以RK(C)为Key取表的值给寄存器
        // GETTABLE
        // A B C    R(A) := R(B)[RK(C)]
        internal static void GetTable(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(c);
            ls.GetTable(b);
            ls.Replace(a);
        }

        // 
        // SETTABLE
        // A B C    R(A)[RK(B)] := RK(C)
        internal static void SetTable(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(b);
            ls.GetRK(c);
            ls.SetTable(a);
        }

        private const int LFIELDS_PER_FLUSH = 50;

        // 批量设置数组元素
        // A B C    R(A)[(C-1)*FPF+i] := R(A+i), 1 <= i <= B
        internal static void SetList(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            c = (c > 0) ? c - 1 : new Instruction(ls.Fetch()).Ax();

            var isZeroB = b == 0;
            if (isZeroB)
            {
                b = (int)ls.ToInteger(-1) - a - 1;
                ls.Pop(1);
            }

            ls.Check(1);
            var idx = (long)(c * LFIELDS_PER_FLUSH);
            for (var j = 1; j <= b; j++)
            {
                idx++;
                ls.PushV(a + j);
                ls.SetI(a, idx);
            }

            if (isZeroB)
            {
                for (var j = ls.RegisterCount() + 1; j <= ls.GetTop(); j++)
                {
                    idx++;
                    ls.PushV(j);
                    ls.SetI(a, idx);
                }

                // clear stack
                ls.SetTop(ls.RegisterCount());
            }
        }

        // CLOSURE
        // R(A) := closure(KPROTO[Bx])
        internal static void Closure(Instruction i, LuaState ls)
        {
            var t = i.ABx();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[ABx:{a}|{b}]");
            ls.LoadProto(b);
            ls.Replace(a);
        }

        // CALL
        // R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
        internal static void Call(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            // println(":::"+ ls.StackToString())
            var nArgs = PushFuncAndArgs(a, b, ls);
            ls.Call(nArgs, c - 1);
            PopResults(a, c, ls);
        }

        // 
        private static int PushFuncAndArgs(int a, int b, LuaState ls)
        {
            if (b >= 1)
            {
                ls.Check(b);
                for (var i = a; i < a + b; i++)
                {
                    ls.PushV(i);
                }

                return b - 1;
            }

            FixStack(a, ls);
            return ls.GetTop() - ls.RegisterCount() - 1;
        }

        // 
        private static void FixStack(int a, LuaState ls)
        {
            var x = (int)ls.ToInteger(-1);
            ls.Pop(1);

            ls.Check(x - a);
            for (var i = a; i < x; i++)
            {
                ls.PushV(i);
            }

            ls.Rotate(ls.RegisterCount() + 1, x - a);
        }

        // 
        private static void PopResults(int a, int c, LuaState ls)
        {
            if (c == 1)
            {
                return;// no results
            }
            
            if (c > 1)
            {
                for (var i = a + c - 2; i >= a; i--)
                {
                    ls.Replace(i);
                }
            }
            else
            {
                // leave results on stack
                ls.Check(1);
                ls.Push(a);
            }
        }

        // return R(A), ... ,R(A+B-2)
        internal static void Return(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[ABC:{a}|{b}]");
            if (b == 1)
            {
                return;// no return values
            }
            
            if (b > 1)
            {
                // b-1 return values
                ls.Check(b - 1);
                for (var j = a; j <= a + b - 2; j++)
                {
                    ls.PushV(j);
                }
            }
            else
            {
                FixStack(a, ls);
            }
        }

        // R(A), R(A+1), ..., R(A+B-2) = vararg
        internal static void Vararg(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[ABC:{a}|{b}]");
            if (b != 1)
            {
                // b == 0 or b > 1
                ls.LoadVararg(b - 1);
                PopResults(a, b, ls);
            }
        }

        // return R(A)(R(A+1), ... ,R(A+B-1))
        internal static void TailCall(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            System.Console.Write($"[ABC:{a}|{b}]");
            // todo: optimize tail call!
            var c = 0;
            var nArgs = PushFuncAndArgs(a, b, ls);
            ls.Call(nArgs, c - 1);
            PopResults(a, c, ls);
        }

        // 
        // R(A+1) := R(B); R(A) := R(B)[RK(C)]
        internal static void Self(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.Copy(b, a + 1);
            ls.GetRK(c);
            ls.GetTable(b);
            ls.Replace(a);
        }

        // 
        // 
        internal static void GetUpval(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.Copy(GetUpvalueIndex(b), a);
        }

        // 
        // 
        internal static void SetUpval(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            System.Console.Write($"[ABC:{a}|{b}]");
            ls.Copy(a, GetUpvalueIndex(b));
        }

        // 
        // 
        internal static void GetTabUp(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(c);
            ls.GetTable(GetUpvalueIndex(b));
            ls.Replace(a);
        }

        // 
        // 
        internal static void SetTabUp(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;
            var c = t.Item3;
            System.Console.Write($"[ABC:{a}|{b}|{c}]");
            ls.GetRK(b);
            ls.GetRK(c);
            var idx = GetUpvalueIndex(a);
            ls.SetTable(idx);
        }

        private static int GetUpvalueIndex(int idx)
        {
            return StateReg.LUA_REGISTRY_INDEX - idx;
        }

        // 对暂未实现的指令，可以先执行此函数，以求编译通过编译测试已实现的指令函数
        internal static void Func(Instruction i, LuaState ls)
        {
            Console.WriteLine($"Warning: instruction [{i.OpName()}] not implement");
        }

        // 
        private static int Int2Fb(int x)
        {
            if (x < 8)
            {
                return x;
            }

            var e = 0;
            for (; x >= (8 << 4);)
            {
                x = (x + 0xf) >> 4;
                e += 4;
            }

            for (; x >= (8 << 1);)
            {
                x = (x + 1) >> 1;
                e++;
            }

            return ((e + 1) << 3) | (x - 8);
        }

        // 
        private static int Fb2Int(int x)
        {
            if (x < 8)
            {
                return x;
            }
            else
            {
                return ((x & 7 + 8)) << ((x >> 3) - 1);
            }
        }
    }
}
