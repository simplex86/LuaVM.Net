using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    internal class Chunk
    {
        internal static byte[] Dump(BinaryChunk chunk)
        {
            return null;
        }

        // 反编译字节码
        internal static Prototype Undump(byte[] bytes)
        {
            ChunkReader reader = new ChunkReader(bytes);
            CheckHeader(reader);
            reader.ReadByte();
            return reader.ReadProto("");
        }

        internal static void CheckHeader(ChunkReader reader)
        {
            if (Convert.Bytes2String(reader.ReadBytes(4)) != ChunkValues.LUA_SIGNATURE)
            {
                Console.WriteLine("not a precompiled chunk!");
            }

            if (reader.ReadByte() != ChunkValues.LUAC_VERSION)
            {
                Console.WriteLine("version mismatch!");
            }

            if (reader.ReadByte() != ChunkValues.LUAC_FORMAT)
            {
                Console.WriteLine("format mismatch!");
            }

            if (Convert.Bytes2String(reader.ReadBytes(6)) != ChunkValues.LUAC_DATA)
            {
                Console.WriteLine("corrupted!");
            }

            if (reader.ReadByte() != ChunkValues.CINT_SIZE)
            {
                Console.WriteLine("int size mismatch!");
            }

            var b = reader.ReadByte();
            if (b != ChunkValues.CSIZET_SIZE_32 && b != ChunkValues.CSIZET_SIZE_64)
            {
                Console.WriteLine("size_t size mismatch!");
            }

            if (reader.ReadByte() != ChunkValues.INSTRUCTION_SIZE)
            {
                Console.WriteLine("instruction size mismatch!");
            }

            if (reader.ReadByte() != ChunkValues.LUA_INTEGER_SIZE)
            {
                Console.WriteLine("lua_Integer size mismatch!");
            }

            if (reader.ReadByte() != ChunkValues.LUA_NUMBER_SIZE)
            {
                Console.WriteLine("lua_Number size mismatch!");
            }

            if (reader.ReadLuaInteger() != ChunkValues.LUAC_INT)
            {
                Console.WriteLine("endianness mismatch!");
            }

            if (!reader.ReadLuaNumber().Equals(ChunkValues.LUAC_NUM))
            {
                Console.WriteLine("float format mismatch!");
            }
        }
    }

    internal struct BinaryChunk
    {
        // 头部
        Header header;
        // 主函数中upvalue的数量
        byte sizeOfUpvalues;
        //主函数原型
        Prototype mainFunc;
    }

    internal struct Header
    {
        // 签名
        byte[] signature;
        // 版本号
        byte version;
        // 格式号
        byte format;
        // LUAC_DATA
        byte[] luacData;
        // int所占字节数
        byte sizeOfCint;
        // size_t所占字节数
        byte sizeOfSizet;
        // lua虚拟机指令所占字节数
        byte sizeOfInstruction;
        // lua整数所占字节数
        byte sizeOfLuaInteger;
        // lua浮点数所占字节数
        byte sizeOfLuaNumber;
        // 固定为0x5678，用于检查大小端
        long luacInteger;
        // 固定为370.5，用于检查浮点数格式
        double luacNumber;
    }

    public class Prototype
    {
        // 源文件名
        internal string source;
        // 起始行号
        internal uint lineDefined;
        // 结束行号
        internal uint lastLineDefined;
        // 固定参数的个数
        internal byte numParams;
        // 是否有可变参数，0：没有；1：有
        internal byte isVararg;
        // 最大寄存器数量
        internal byte maxStackSize;
        // 指令表
        internal uint[] code;
        // 常量表
        internal LuaValue[] constants;
        // upvalue描述表
        internal UpvalueDesc[] upvalues;
        // 子函数原型表
        internal Prototype[] protos;
        // 行号表
        internal uint[] lineInfo;
        // 局部变量表
        internal LocalVariate[] localVars;
        // upvalue名列表
        internal string[] upvalueNames;
    }

    internal struct UpvalueDesc
    {
        internal byte instack;
        internal byte idx;
    }

    internal struct LocalVariate
    {
        internal string name;
        internal uint startPC;
        internal uint endPC;
    }
}
