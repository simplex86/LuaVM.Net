using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Net.Core
{
    // chunk各字段的（固定）值
    internal static class ChunkValues
    {
        internal const string LUA_SIGNATURE = "\x1bLua";
        internal const byte LUAC_VERSION = 0x53;
        internal const byte LUAC_FORMAT = 0;
        internal const string LUAC_DATA = "\x19\x93\r\n\x1a\n";
        internal const uint CINT_SIZE = 4;
        internal const uint CSIZET_SIZE_32 = 4;
        internal const uint CSIZET_SIZE_64 = 8;
        internal const uint INSTRUCTION_SIZE = 4;
        internal const uint LUA_INTEGER_SIZE = 8;
        internal const uint LUA_NUMBER_SIZE = 8;
        internal const ushort LUAC_INT = 0x5678;
        internal const double LUAC_NUM = 370.5;

        internal const byte TAG_NIL = 0x00;
        internal const byte TAG_BOOLEAN = 0x01;
        internal const byte TAG_NUMBER = 0x03;
        internal const byte TAG_INTEGER = 0x13;
        internal const byte TAG_SHORT_STR = 0x04;
        internal const byte TAG_LONG_STR = 0x14;
    }

    // 指令编码模式
    internal static class InstructionMode
    {
        internal const byte IABC = 0;
        internal const byte IABx = 1;
        internal const byte IAsBx = 2;
        internal const byte IAx = 3;
    }

    // 操作数
    internal static class OperationArgs
    {
        internal const byte N = 0;
        internal const byte U = 1;
        internal const byte R = 2;
        internal const byte K = 3;
    }
}
