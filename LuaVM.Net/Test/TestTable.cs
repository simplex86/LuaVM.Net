using System;
using System.Collections.Generic;
using LuaVM.Net.Core;

namespace LuaVM.Net
{
    class TestTable
    {
        public void Do(string filename)
        {
            // var t = new LuaTable(0, 0);
            // Console.WriteLine($"before,\tlen = {t.Len()}");
            // var k1 = new LuaValue(1); t.Set(k1, new LuaValue("a"));
            // var k2 = new LuaValue(2); t.Set(k2, new LuaValue("b"));
            // Console.WriteLine($"after,\tlen = {t.Len()}");
            // 
            // var ka = new LuaValue("a"); t.Set(ka, new LuaValue("aa"));
            // var kb = new LuaValue("b"); t.Set(kb, new LuaValue("bb"));
            // var kc = new LuaValue(1.234); t.Set(kc, new LuaValue("1.234"));
            // 
            // t.Set(ka, new LuaValue("aaa"));
            // 
            // var va = t.Get(ka);
            // Console.WriteLine($"key = {ka.GetString()},\tval = {va.GetString()}");
            // var vb = t.Get(kb);
            // Console.WriteLine($"key = {kb.GetString()},\tval = {vb.GetString()}");
            // var vc = t.Get(kc);
            // Console.WriteLine($"key = {kc.GetFloat()},\tval = {vc.GetString()}");

            Console.WriteLine("table test:");
            var proto = Test.LoadLuac(filename);
            Test.RunLuac(proto);
            Console.WriteLine("table test done!");
        }
    }
}
