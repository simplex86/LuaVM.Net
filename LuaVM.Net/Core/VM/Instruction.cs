//#define _SHOW_INSTRUCTION_INFO_
using System;

namespace LuaVM.Net.Core
{
    class Instruction
    {
        private uint data = 0;

        private const int MAX_ARG_Bx  = (1 << 18) - 1;
        private const int MAX_ARG_sBx = MAX_ARG_Bx >> 1;

        public Instruction(uint data)
        {
            this.data = data;
        }

        // 获取操作码
        public byte OpCode()
        {
            return (byte)(data & 0x3F);
        }

        // 获取操作码名字
        public string OpName()
        {
            var c = OpCode();
            return OperationCodes.Get(c).name;
        }

        // 获取操作码编码模式
        public byte OpMode()
        {
            var c = OpCode();
            return OperationCodes.Get(c).opMode;
        }

        public byte OpBMode()
        {
            var c = OpCode();
            return OperationCodes.Get(c).argBMode;
        }

        public byte OpCMode()
        {
            var c = OpCode();
            return OperationCodes.Get(c).argCMode;
        }

        // 从iABC模式指令中获取参数
        public Tuple<int, int, int> ABC()
        {
            var a = (int)((data >> 6)  & 0xFF);
            var b = (int)((data >> 23) & 0x1FF);
            var c = (int)((data >> 14) & 0x1FF);

            return Tuple.Create(a, b, c);
        }

        // 从iABx模式指令中获取参数
        public Tuple<int, int> ABx()
        {
            var a = (int)((data >> 6) & 0xFF);
            var b = (int)(data >> 14);

            return Tuple.Create(a, b);
        }

        // 从iAsBx模式指令中获取参数
        public Tuple<int, int> AsBx()
        {
            var n = ABx();
            var a = n.Item1;
            var b = n.Item2 - MAX_ARG_sBx;

            return Tuple.Create(a, b);
        }

        // 从iAx模式指令中获取参数
        public int Ax()
        {
            return (int)(data >> 6);
        }

        public void Execute(LuaState state)
        {
            var code = OpCode();
            var invoke = OperationCodes.Get(code).invoke;

            if (invoke != null)
            {
#if _SHOW_INSTRUCTION_INFO_
                System.Console.Write($"[{OpName()}]");
#endif
                invoke(this, state);
#if _SHOW_INSTRUCTION_INFO_
                state.PrintStack("\nstack: ");
                System.Console.WriteLine();
#endif
            }
            else
            {
                var name = OpName();
                Error.Commit($"instruction [{name}] execute exception!");
            }
        }
    }
}
