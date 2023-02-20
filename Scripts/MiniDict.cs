using System;
using Unity.Collections.LowLevel.Unsafe;

namespace ECFSM
{
    public class MiniDict2<Key>
    {
        // 键值对总数
        private int N;
        public int Count => N;
        // 容量
        private int M = 16;
        private Key[] keys;
        private object[] values;

        public MiniDict2()
        {
            keys = new Key[M];
            values = new object[M];
        }

        // 指定容量
        public MiniDict2(int cap)
        {
            M = cap;
            keys = new Key[M];
            values = new object[M];
        }

        // 不要从索引器删除, key 不能为 null
        public ref object this[Key key]
        {
            get
            {
                if (N > M / 2) Resize(M * 2);

                int i = 0;
                for (i = Hash(key); keys[i] != null; i = (i + 1) % M)
                {
                    if (Object.Equals(keys[i], key))
                    {
                        return ref values[i];
                    }
                }
                keys[i] = key;
                N += 1;
                return ref values[i];
            }
        }

        // 计算 hash
        private int Hash(Key key)
        {
            return (key.GetHashCode() & 0x7fffffff) % M;
        }

        // 更改容量, 用于扩容或元素太少
        private void Resize(int cap)
        {
            MiniDict2<Key> newDict = new MiniDict2<Key>(cap);
            for (int i = 0; i < M; i++)
            {
                if (keys[i] != null)
                {
                    newDict[keys[i]] = values[i];
                }
            }
            keys = newDict.keys;
            values = newDict.values;
            M = newDict.M;
        }

        // 增改
        public void Set<T>(Key key, T value)
        {
            if (N > M / 2) Resize(M * 2);

            int i = 0;
            for (i = Hash(key); keys[i] != null; i = (i + 1) % M)
            {
                if (Object.Equals(keys[i], key))
                {
                    values[i] = new MyClass<T>(value);
                    return;
                }
            }
            keys[i] = key;
            values[i] = new MyClass<T>(value);
            N += 1;
        }

        //// 增改
        //public void Set(Key key, object value)
        //{
        //    if (N > M / 2) Resize(M * 2);

        //    int i = 0;
        //    for (i = Hash(key); keys[i] != null; i = (i + 1) % M)
        //    {
        //        if (Object.Equals(keys[i], key))
        //        {
        //            values[i] = new MyClass<object>(value);
        //            return;
        //        }
        //    }
        //    keys[i] = key;
        //    values[i] = new MyClass<object>(value);
        //    N += 1;
        //}

        // 查
        public ref MyClass<T> Get<T>(Key key)
        {
            for (int i = Hash(key); keys[i] != null; i = (i + 1) % M)
                if (Object.Equals(keys[i], key))
                    return ref UnsafeUtility.As<object, MyClass<T>>(ref values[i]);
            throw null;
        }

        //public ref object Get(Key key)
        //{
        //    for (int i = Hash(key); keys[i] != null; i = (i + 1) % M)
        //        if (Object.Equals(keys[i], key))
        //            return ref values[i];
        //    throw null;
        //}

        // 删
        public void Delete(Key key)
        {
            if (!Contains(key)) return;
            int i = Hash(key);
            while (!Object.Equals(keys[i], key))
                i = (i + 1) % M;
            keys[i] = default(Key);
            values[i] = default(MyClass<object>);
            i = (i + 1) % M;
            while (keys[i] != null)
            {
                Key keyToRedo = keys[i];
                object valueToRedo = values[i];
                keys[i] = default(Key);
                values[i] = default(MyClass<object>);
                N -= 1;
                Set(keyToRedo, valueToRedo);
                i = (i + 1) % M;
            }
            N -= 1;
            if (N > 0 && N == M / 8) Resize(M / 2);
        }

        // 判断 key 是否存在
        public bool Contains(Key key)
        {
            for (int i = Hash(key); keys[i] != null; i = (i + 1) % M)
                if (Object.Equals(keys[i], key))
                    return true;
            return false;
        }
    }

}