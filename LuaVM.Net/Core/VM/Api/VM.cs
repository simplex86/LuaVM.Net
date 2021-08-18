using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal static class VM
    {
        // MOVE
        internal static void Move(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            ls.Copy(a, b);
        }

        // JMP
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

        // LOADNIL
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

        // LOADBOOL
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

        // LOADK
        internal static void LoadK(Instruction i, LuaState ls)
        {
            var t = i.ABx();
            var a = t.Item1 + 1;
            var b = t.Item2;

            ls.PushConst(b);
            ls.Replace(a);
        }

        // LOADKX
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

        internal static void Add(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPADD);
        }

        internal static void Sub(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSUB);
        }

        internal static void Mul(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPMUL);
        }

        internal static void Mod(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPMOD);
        }

        internal static void Pow(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPPOW);
        }

        internal static void Div(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPDIV);
        }

        internal static void IDiv(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPIDIV);
        }

        internal static void BAnd(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBAND);
        }

        internal static void BOr(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBOR);
        }

        internal static void BXor(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPBXOR);
        }

        internal static void Shl(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSHL);
        }

        internal static void Shr(Instruction i, LuaState ls)
        {
            BinaryOp(i, ls, Operations.LUA_OPSHR);
        }

        internal static void Unm(Instruction i, LuaState ls)
        {
            UnaryOp(i, ls, Operations.LUA_OPUNM);
        }

        internal static void BNot(Instruction i, LuaState ls)
        {
            UnaryOp(i, ls, Operations.LUA_OPBNOT);
        }

        // 长度
        internal static void Len(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;

            ls.Len(b);
            ls.Replace(a);
        }

        // 连接
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
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c = abc.Item3 + 1;

            ls.PushRK(b);
            ls.PushRK(c);
            if (ls.Compare(-2, -1, op) != (a != 0))
            {
                ls.AddPC(1);
            }

            ls.Pop(2);
        }

        internal static void EQ(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPEQ);
        }

        internal static void LT(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPLT);
        }

        internal static void LE(Instruction i, LuaState ls)
        {
            Compare(i, ls, Operations.LUA_OPLE);
        }

        // NOT
        internal static void Not(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            //var c = t.Item3 + 1;

            ls.Push(!ls.ToBoolean(b));
            ls.Replace(a);
        }

        // TEST
        internal static void Test(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3 + 1;

            if (ls.ToBoolean(a) != (c != 0))
            {
                ls.AddPC(1);
            }
        }

        // TESTSET
        internal static void TestSet(Instruction i, LuaState ls)
        {
            var t = i.ABC();
            var a = t.Item1 + 1;
            var b = t.Item2 + 1;
            var c = t.Item3 + 1;

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

        internal static void Func(Instruction i, LuaState ls)
        {

        }
    }
}
