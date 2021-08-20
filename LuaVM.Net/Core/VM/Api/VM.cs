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
            ls.Copy(a, b);
        }

        // 无条件跳转
        // JMP
        // sBx      pc += sBx
        internal static void Jump(Instruction i, LuaState ls)
        {
            var t = i.AsBx();
            var a = t.Item1;
            var b = t.Item2;

            ls.AddPC(b);
            if (a != 0)
            {
                Error.Commit("todo");
            }
        }

        // 加载nil给寄存器
        // LOADNIL
        // A B      R(A), R(A+1), ..., R(A+B) := nil
        internal static void LoadNil(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2;

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

            ls.PushConst(b);
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

            ls.PushConst(b);
            ls.Replace(a);
        }

        // 一元运算
        private static void UnaryOp(Instruction i, LuaState ls, int op)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;

            ls.PushX(b);
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

            ls.PushRK(b);
            ls.PushRK(c);
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

            var n = c - b + 1;
            ls.Check(n);

            for (var k = b; k <= c; k++)
            {
                ls.PushX(k);
            }

            ls.Concat(n);
            ls.Replace(a);
        }

        // 比较
        private static void Compare(Instruction i, LuaState ls, int op)
        {
            var abc = i.ABC();
            var a = abc.Item1;
            var b = abc.Item2;
            var c = abc.Item3;

            ls.PushRK(b);
            ls.PushRK(c);
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

            ls.PushX(a);
            ls.PushX(a + 2);
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

            // R(A)+=R(A+2);
            ls.PushX(a + 2);
            ls.PushX(a);
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

            ls.PushRK(c);
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

            ls.PushRK(b);
            ls.PushRK(c);
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

            c = (c > 0) ? c - 1 : new Instruction(ls.Fetch()).Ax();

            var idx = (long)c * LFIELDS_PER_FLUSH;
            for (var k = 1; k <= b; k++)
            {
                idx++;
                ls.PushX(a + k);
                ls.SetI(a, idx);
            }
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
