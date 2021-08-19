using System;
using System.Text;

namespace LuaVM.Net.Core
{
    // 获取各种类型数据的Hash值
    // TODO  目前暂时用.net自带的算法，后面有空了再来写
    internal static class Hash
    {
        public static int Get(bool b)
        {
            return b.GetHashCode();
        }

        public static int Get(long n)
        {
            return n.GetHashCode();
        }

        public static int Get(double n)
        {
            return n.GetHashCode();
        }

        public static int Get(string s)
        {
            return s.GetHashCode();
        }
    }
}
