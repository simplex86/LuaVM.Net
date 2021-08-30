using System;
using System.Collections.Generic;
using System.Text;

namespace LuaVM.Net.Core
{
    static class Error
    {
        public static void Commit(string err)
        {
            //Console.WriteLine(err);
            throw new Exception(err);
        }
    }
}
