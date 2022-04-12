using System;
using System.Collections.Generic;
using System.Linq;

namespace LuaVM.Net.Core
{
    class LuaTable
    {
        private List<LuaValue> arr;
        private Dictionary<int, LuaValue> map;

        public LuaTable(int n, int m)
        {
            if (n > 0)
            {
                arr = new List<LuaValue>(n);
            }
            else
            {
                arr = new List<LuaValue>();
            }

            if (m > 0)
            {
                map = new Dictionary<int, LuaValue>(m);
            }
            else
            {
                map = new Dictionary<int, LuaValue>();
            }
        }

        public LuaValue Get(LuaValue key)
        {
            if (key.IsInteger())
            {
                var i = (int)key.GetInteger() - 1;
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
            if (key == null)
            {
                Error.Commit("");
                return;
            }

            if (key.IsNil())
            {
                Error.Commit("");
                return;
            }

            if (val.IsFloat() && double.IsNaN(val.GetFloat()))
            {
                Error.Commit("");
                return;
            }

            if (key.IsInteger())
            {
                var i = (int)key.GetInteger();
                if (i > 0)
                {
                    var len = (arr == null) ? 0 : arr.Count;
                    if (i <= len)
                    {
                        arr[i - 1] = val;
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
                if (arr[i] == null)
                {
                    //Array.Copy(arr, 0, arr, 0, i);
                }
            }
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
