using System;
using System.Text;

namespace LuaVM.Net.Core
{
    public class ChunkReader
    {
        private byte[] bytes = null;
        private int index = 0;

        public ChunkReader(byte[] bytes)
        {
            this.bytes = bytes;
            this.index = 0;
        }

        public byte ReadByte()
        {
            byte b = bytes[index];
            Skip(1);

            return b;
        }

        public uint ReadUInt32()
        {
            var b = new byte[4];
            Array.ConstrainedCopy(bytes, index, b, 0, b.Length);
            Skip(b.Length);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            return BitConverter.ToUInt32(b, 0);
        }

        public ulong ReadUInt64()
        {
            var b = new byte[8];
            Array.ConstrainedCopy(bytes, index, b, 0, b.Length);
            Skip(b.Length);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            return BitConverter.ToUInt64(b, 0);
        }

        public byte[] ReadBytes(uint n)
        {
            var b = new byte[n];
            Array.ConstrainedCopy(bytes, index, b, 0, (int)n);
            Skip((int)n);

            return b;
        }

        public long ReadLuaInteger()
        {
            return (long)ReadUInt64();
        }

        public double ReadLuaNumber()
        {
            return BitConverter.Int64BitsToDouble((long)ReadUInt64());
        }

        public Prototype ReadProto(string parentSource)
        {
            var source = ReadString();
            if (string.IsNullOrEmpty(source))
            {
                source = parentSource;
            }

            return new Prototype
            {
                source          = source,
                lineDefined     = ReadUInt32(),
                lastLineDefined = ReadUInt32(),
                numParams       = ReadByte(),
                isVararg        = ReadByte(),
                maxStackSize    = ReadByte(),
                code            = ReadCode(),
                constants       = ReadConstants(),
                upvalues        = ReadUpvalues(),
                protos          = ReadProtos(source),
                lineInfo        = ReadLineInfo(),
                localVars       = ReadLocalVars(),
                upvalueNames    = ReadUpvalueNames()
            };
        }

        private void Skip(int n)
        {
            index += n;
        }

        private string[] ReadUpvalueNames()
        {
            var names = new string[ReadUInt32()];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = ReadString();
            }

            return names;
        }

        private LocalVariate[] ReadLocalVars()
        {
            var vars = new LocalVariate[ReadUInt32()];
            for (var i = 0; i < vars.Length; i++)
            {
                vars[i] = new LocalVariate
                {
                    name    = ReadString(),
                    startPC = ReadUInt32(),
                    endPC   = ReadUInt32()
                };
            }

            return vars;
        }

        private uint[] ReadLineInfo()
        {
            var lineInfo = new uint[ReadUInt32()];
            for (var i = 0; i < lineInfo.Length; i++)
            {
                lineInfo[i] = ReadUInt32();
            }

            return lineInfo;
        }

        private Prototype[] ReadProtos(string parentSource)
        {
            var protos = new Prototype[ReadUInt32()];
            for (var i = 0; i < protos.Length; i++)
            {
                protos[i] = ReadProto(parentSource);
            }

            return protos;
        }

        private Upvalue[] ReadUpvalues()
        {
            var upvalues = new Upvalue[ReadUInt32()];
            for (var i = 0; i < upvalues.Length; i++)
            {
                upvalues[i] = new Upvalue
                {
                    instack = ReadByte(),
                    idx     = ReadByte()
                };
            }

            return upvalues;
        }

        private object[] ReadConstants()
        {
            var constants = new object[ReadUInt32()];
            for (var i = 0; i < constants.Length; i++)
            {
                constants[i] = ReadConstant();
            }

            return constants;
        }

        private object ReadConstant()
        {
            switch (ReadByte())
            {
                case ChunkValues.TAG_NIL:       return null;
                case ChunkValues.TAG_BOOLEAN:   return ReadByte() != 0;
                case ChunkValues.TAG_INTEGER:   return ReadLuaInteger();
                case ChunkValues.TAG_NUMBER:    return ReadLuaNumber();
                case ChunkValues.TAG_SHORT_STR: return ReadString();
                case ChunkValues.TAG_LONG_STR:  return ReadString();
                default: throw new Exception("corrupted!");
            }
        }

        private uint[] ReadCode()
        {
            var code = new uint[ReadUInt32()];
            for (var i = 0; i < code.Length; i++)
            {
                code[i] = ReadUInt32();
            }

            return code;
        }

        private string ReadString()
        {
            var size = (uint)ReadByte();
            if (size == 0)
            {
                return "";
            }

            if (size == 0xFF)
            {
                size = (uint)ReadUInt64();
            }

            var b = ReadBytes(size - 1);
            return Convert.Bytes2String(b);
        }
    }
}
