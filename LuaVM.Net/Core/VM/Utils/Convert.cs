using System;
using System.Linq;
using System.Text;

namespace LuaVM.Net.Core
{
    public static class Convert
    {
        public static string Bytes2String(byte[] bytes)
        {
            return Encoding.UTF7.GetString(bytes);
        }
    }
}
