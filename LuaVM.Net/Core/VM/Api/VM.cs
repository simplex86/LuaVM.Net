using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal static class VM
    {
        // MOVE
        // A B      R(A) := R(B)
        internal static void Move(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;
            ls.Copy(b, a);
        }

        // JMP
        // sBx      pc += sBx
        internal static void Jump(Instruction i, LuaState ls)
        {
            var (a, b) = i.AsBx();

            ls.AddPC(b);
            if (a != 0)
            {
                ls.CloseUpvalues(a);
            }
        }

        // LOADNIL
        // A B      R(A), R(A+1), ..., R(A+B) := nil
        internal static void LoadNil(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;

            ls.Push();

            for (int k = a; k<= a+b; k++)
            {
                ls.Copy(-1, k);
            }
            ls.Pop(1);
        }

        // LOADBOOL
        // A B C    R(A) := (Bool)B; if(C) pc++
        internal static void LoadBoolean(Instruction i, LuaState ls)
        {
            var(a, b, c) = i.ABC();
            a += 1;

            ls.Push(b != 0);
            ls.Replace(a);
            if (c != 0)
            {
                ls.AddPC(1);
            }
        }

        // LOADK
        // A Bx     R(A) := Kst(Bx)
        internal static void LoadK(Instruction i, LuaState ls)
        {
            var (a, b) = i.ABx();
            a += 1;

            ls.PushConst(b);
            ls.Replace(a);
        }

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
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.PushX(b);
            ls.Arithmetic(op);
            ls.Replace(a);
        }

        // 二元运算
        private static void BinaryOp(Instruction i, LuaState ls, int op)
        {
            var (a, b, c) = i.ABC();
            a += 1;

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
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.Len(b);
            ls.Replace(a);
        }

        // CONCAT
        // A B C	R(A) := R(B).. ... ..R(C)
        internal static void Concat(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;
            c += 1;

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
            var (a, b, c) = i.ABC();

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
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.Push(!ls.ToBoolean(b));
            ls.Replace(a);
        }

        // TEST
        // A C	    if not (R(A) <=> C) then pc++
        internal static void Test(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;

            if (ls.ToBoolean(a) != (c != 0))
            {
                ls.AddPC(1);
            }
        }

        // TESTSET
        // A B C	if (R(B) <=> C) then R(A) := R(B) else pc++
        internal static void TestSet(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

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
            var (a, b) = i.AsBx();
            a += 1;

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
            var (a, b) = i.AsBx();
            a += 1;

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

        // CLOSURE
        // R(A) := closure(KPROTO[Bx])
        internal static void Closure(Instruction i, LuaState ls)
        {
            var (a, b) = i.ABx();
            a += 1;

            ls.LoadProto(b);
            ls.Replace(a);
        }

        // CALL
        // R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
        internal static void Call(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            
            var nArgs = PushFuncAndArgs(a, b, ls);
            ls.Call(nArgs, c - 1);
            PopResults(a, c, ls);
        }

        // RETURN
        // return R(A), ... ,R(A+B-2)
        internal static void Return(Instruction i, LuaState ls)
        {
            var (a, b, _) = i.ABC();
            a += 1;

            if (b == 1)
            {
                // no return values
            }
            else if (b > 1)
            {
                // b-1 return values
                ls.Check(b - 1);
                for (var j = a; j <= a + b - 2; j++)
                {
                    ls.PushX(j);
                }
            }
            else
            {
                FixStack(a, ls);
            }
        }

        // VARARG
        // R(A), R(A+1), ..., R(A+B-2) = vararg
        internal static void Vararg(Instruction i, LuaState ls)
        {
            var (a, b, _) = i.ABC();
            a += 1;

            if (b != 1)
            {
                // b==0 or b>1
                ls.LoadVararg(b - 1);
                PopResults(a, b, ls);
            }
        }

        // TAILCALL
        // return R(A)(R(A+1), ... ,R(A+B-1))
        internal static void TailCall(Instruction i, LuaState ls)
        {
            var (a, b, _) = i.ABC();
            a += 1;

            // TODO XH: optimize tail call!
            var c = 0;
            var nArgs = PushFuncAndArgs(a, b, ls);
            ls.Call(nArgs, c - 1);
            PopResults(a, c, ls);
        }

        // R(A+1) := R(B); R(A) := R(B)[RK(C)]
        internal static void Self(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.Copy(b, a + 1);
            ls.PushRK(c);
            ls.GetTable(b);
            ls.Replace(a);
        }

        private static int PushFuncAndArgs(int a, int b, LuaState ls)
        {
            if (b >= 1)
            {
                ls.Check(b);
                for (var i = a; i < a + b; i++)
                {
                    ls.PushX(i);
                }

                return b - 1;
            }

            FixStack(a, ls);
            return ls.GetTop() - ls.registerCount - 1;
        }

        private static void FixStack(int a, LuaState ls)
        {
            var x = (int)ls.ToInteger(-1);
            ls.Pop(1);

            ls.Check(x - a);
            for (var i = a; i < x; i++)
            {
                ls.PushX(i);
            }

            ls.Rotate(ls.registerCount + 1, x - a);
        }

        private static void PopResults(int a, int c, LuaState ls)
        {
            if (c == 1)
            {
                // no results
            }
            else if (c > 1)
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

        internal static void NewTable(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
 
            ls.CreateTable(Int2Fb(b), Fb2Int(c));
            ls.Replace(a);
        }

        private static int Int2Fb(int x)
        {
            var e = 0;
            if (x < 8)
            {
                return x;
            }

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

        internal static void GetTable(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.PushRK(c);
            ls.GetTable(b);
            ls.Replace(a);
        }

        internal static void SetTable(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;

            ls.PushRK(b);
            ls.PushRK(c);
            ls.SetTable(a);
        }

        private const long LFIELDS_PER_FLUSH = 50;

        internal static void SetList(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;

            if (c > 0)
            {
                c = c - 1;
            }
            else
            {
                c = new Instruction(ls.Fetch()).Ax();
            }

            var iszero = b == 0;
            if (iszero)
            {
                b = (int)ls.ToInteger(-1) - a - 1;
                ls.Pop(1);
            }

            ls.Check(1);
            var idx = c * LFIELDS_PER_FLUSH;
            for (var k = 1; k <= b; k++)
            {
                idx++;
                ls.PushX(a + k);
                ls.SetI(a, idx);
            }

            if (iszero)
            {
                for (var k = ls.registerCount + 1; k <= ls.GetTop(); k++)
                {
                    idx++;
                    ls.PushX(k);
                    ls.SetI(a, idx);
                }

                // clear stack
                ls.SetTop(ls.registerCount);
            }
        }

        //internal static void GetTabUp(Instruction i, LuaState ls)
        //{
        //    var t = i.ABC();
        //    var a = t.Item1 + 1;
        //    var c = t.Item3;

        //    ls.PushGlobalTable();
        //    ls.PushRK(c);
        //    ls.GetTable(-2);
        //    ls.Replace(a);
        //    ls.Pop(1);
        //}

        internal static void GetUpval(Instruction i, LuaState ls)
        {
            var (a, b, _) = i.ABC();
            a += 1;
            b += 1;

            ls.Copy(LuaState.LuaUpvalueIndex(b), a);
        }


        // UpValue[B] := R(A)
        internal static void SetUpval(Instruction i, LuaState ls)
        {
            var (a, b, _) = i.ABC();
            a += 1;
            b += 1;

            ls.Copy(a, LuaState.LuaUpvalueIndex(b));
        }

        // R(A) := UpValue[B][RK(C)]
        internal static void GetTabUp(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;
            b += 1;

            ls.PushRK(c);
            ls.GetTable(LuaState.LuaUpvalueIndex(b));
            ls.Replace(a);
        }

        // UpValue[A][RK(B)] := RK(C)
        internal static void SetTabUp(Instruction i, LuaState ls)
        {
            var (a, b, c) = i.ABC();
            a += 1;

            ls.PushRK(b);
            ls.PushRK(c);
            ls.SetTable(LuaState.LuaUpvalueIndex(a));
        }

        // 对暂未实现的指令，可以先执行此函数，以求编译通过编译测试已实现的指令函数
        internal static void Func(Instruction i, LuaState ls)
        {
            Console.WriteLine($"Warning: instruction [{i.OpName()}] not implement");
        }
    }
}
