using System;
using System.IO;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestUndump
    {
        // 加载lua字节码文件
        public void Do(string filename)
        {
            Console.WriteLine("undump test:");
            var proto = Test.LoadLuac(filename);
            PrintProto(proto);
            Console.WriteLine("undump test done!");
        }

        private void PrintProto(Prototype proto)
        {
            PrintHeader(proto);
            PrintCode(proto);
            PrintDetail(proto);

            foreach (var p in proto.protos)
            {
                PrintProto(p);
            }
        }

        private void PrintHeader(Prototype proto)
        {
            var funcType = proto.lineDefined > 0 ? "function" : "main";
            var varargFlag = proto.isVararg > 0 ? "+" : "";

            Console.Write($"{funcType} <{proto.source}:{proto.lineDefined},{proto.lastLineDefined}> ({proto.code.Length} instructions)\n");
            Console.Write($"{proto.numParams},{varargFlag} params, {proto.maxStackSize} slots, {proto.upvalues.Length} upvalues, ");
            Console.Write($"{proto.localVars.Length} locals, {proto.constants.Length} constants, {proto.protos.Length} functions\n");
        }

        private void PrintCode(Prototype proto)
        {
            for (var pc = 0; pc < proto.code.Length; pc++)
            {
                var c = proto.code[pc];
                var line = "-";
                if (proto.lineInfo.Length > 0)
                {
                    line = proto.lineInfo[pc].ToString();
                }

                var inst = new Instruction(c);
                Console.Write($"\t{pc + 1}\t[{line}]\t{inst.OpName():x8} \t");
                PrintOperands(inst);
                Console.WriteLine();
            }
        }

        private void PrintOperands(Instruction inst)
        {
            switch (inst.OpMode())
            {
                case InstructionMode.IABC:
                    var abc = inst.ABC();
                    Console.Write($"{abc.Item1:D}");
                    if (inst.OpBMode() != OperationArgs.N)
                    {
                        if (abc.Item2 > 0xFF)
                        {
                            Console.Write($"  {-1 - (abc.Item2 & 0xFF):D}");
                        }
                        else
                        {
                            Console.Write($"  {abc.Item2:D}");
                        }
                    }

                    if (inst.OpCMode() != OperationArgs.N)
                    {
                        if (abc.Item3 > 0xFF)
                        {
                            Console.Write($"  {-1 - (abc.Item3 & 0xFF):D}");
                        }
                        else
                        {
                            Console.Write($"  {abc.Item3:D}");
                        }
                    }

                    break;
                case InstructionMode.IABx:
                    var aBx = inst.ABx();
                    Console.Write($"{aBx.Item1:D}");
                    if (inst.OpBMode() == OperationArgs.K)
                    {
                        Console.Write($"  {-1 - aBx.Item2:D}");
                    }
                    else if (inst.OpBMode() == OperationArgs.U)
                    {
                        Console.Write($"  {aBx.Item2:D}");
                    }

                    break;
                case InstructionMode.IAsBx:
                    var asBx = inst.AsBx();
                    Console.Write($"{asBx.Item1:D} {asBx.Item2:D}");
                    break;
                case InstructionMode.IAx:
                    var ax = inst.Ax();
                    Console.Write($"{-1 - ax:D}");
                    break;
            }
        }

        private void PrintDetail(Prototype proto)
        {
            Console.Write($"constants ({proto.constants.Length}):\n");
            for (var i = 0; i < proto.constants.Length; i++)
            {
                var k = proto.constants[i];
                Console.Write($"\t{i + 1}\t{ConstantToString(k)}\n");
            }

            Console.Write($"locals ({proto.localVars.Length}):\n");
            for (var i = 0; i < proto.localVars.Length; i++)
            {
                var locVar = proto.localVars[i];
                Console.Write($"\t{i}\t{locVar.name}\t{locVar.startPC + 1}\t{locVar.endPC + 1}\n");
            }

            Console.Write($"upvalues ({proto.upvalues.Length}):\n");
            for (var i = 0; i < proto.upvalues.Length; i++)
            {
                var upval = proto.upvalues[i];
                Console.Write($"\t{i}\t{GetUpvalueName(proto, i)}\t{upval.instack}\t{upval.idx}\n");
            }
        }

        private string GetUpvalueName(Prototype proto, int idx)
        {
            return proto.upvalueNames.Length > 0 ? proto.upvalueNames[idx] : "-";
        }

        private object ConstantToString(object k)
        {
            if (k == null)
            {
                return "nil";
            }

            switch (k.GetType().Name)
            {
                case "Boolean": return (bool)k;
                case "Double": return (double)k;
                case "Long": return (long)k;
                case "String": return (string)k;
                default: return "?";
            }
        }
    }
}
