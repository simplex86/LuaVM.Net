using System;
using System.Collections.Generic;
using System.Linq;

namespace LuaVM.Net.Core
{
    class LuaTable
    {
        private List<LuaValue> arr;
        private Dictionary<int, LuaValue> map;

        public LuaTable()
            : this(0, 0)
        {

        }
        public LuaTable(int n, int m)
        {
            arr = (n > 0) ? new List<LuaValue>(n) : new List<LuaValue>();
            map = (m > 0) ? new Dictionary<int, LuaValue>(m) : new Dictionary<int, LuaValue>();
        }

        public LuaValue Get(LuaValue key)
        {
            if (key.IsInteger())
            {
                var i = key.GetInteger() - 1;
                if (i >= 0 && i < arr.Count)
                {
                    return arr[(int)i];
                }
            }

            var k = GetHash(key);
            if (map.ContainsKey(k))
            {
                return map[k];
            }

            // 否则，返回nil
            return new LuaValue();
        }

        public void Set(LuaValue key, LuaValue val)
        {
            if (LuaValue.GetType(key) == LuaType.LUA_TNIL)
            {
                Error.Commit("table index is nil!");
                return;
            }

            if (key.IsFloat() && double.IsNaN(key.GetFloat()))
            {
                Error.Commit("table index is NaN!");
                return;
            }

            if (key.IsInteger())
            {
                var i = (int)key.GetInteger();
                if (i > 0)
                {
                    var len = arr.Count;
                    if (i <= len)
                    {
                        arr[i-1] = val;
                        if (i == len && val.IsNil())
                        {
                            ShrinkArray();
                        }
                        return;
                    }

                    if (i == len + 1)
                    {
                        if (!val.IsNil())
                        {
                            arr.Add(val);
                            ExpandArray();
                        }
                        return;
                    }
                }
            }

            var k = GetHash(key);
            if (val.IsNil())
            {
                map.Remove(k);
            }
            else
            {
                if (map.ContainsKey(k))
                {
                    map[k] = val;
                }
                else
                {
                    map.Add(k, val);
                }
            }
        }

        public int Len()
        {
            return arr.Count;
        }

        private void ShrinkArray()
        {
            for (var i = arr.Count - 1; i >= 0; i--)
            {
                if (LuaValue.GetType(arr[i]) == LuaType.LUA_TNIL)
                {
                    ShrinkCopy(i);
                }
            }
        }

        private void ShrinkCopy(int idx)
        {
            for (var i=idx+1; i<arr.Count; i++)
            {
                arr[idx] = arr[i];
            }
            arr.RemoveAt(arr.Count - 1);
        }

        private void ExpandArray()
        {
            for (var i = arr.Count; true; i++)
            {
                var k = Hash.Get(i);

                if (!map.ContainsKey(k))
                    break;

                arr.Add(map[k]);
                map.Remove(k);
            }
        }

        private int GetHash(LuaValue value)
        {
            return LuaValue.GetHash(value);
        }
    }
}
