using System.Collections.Generic;

namespace DictPlus
{
    // 字典里真正存数据的箱子，Variable<T>.value 才是数据本体
    public abstract class Variable
    {
    }

    public class Variable<T> : Variable
    {
        public T value;

        public Variable(T value)
        {
            this.value = value;
        }
    }

    // 用 ref 返回值代替裸指针：拿到 ref 后直接当局部变量读写，
    // 改的就是字典里存的那份，全程安全，不再需要 pin 住 GC 对象。
    public static class DictPlus
    {
        #region Set

        // 值类型：返回 ref，可以紧接着当普通变量用
        public static ref V SetRef<V>(this Dictionary<string, Variable> dict, string key, V value) where V : unmanaged
        {
            if (dict.TryGetValue(key, out Variable v))
            {
                return ref ((Variable<V>)v).value;
            }
            Variable<V> box = new Variable<V>(value);
            dict[key] = box;
            return ref box.value;
        }

        // 引用类型：返回对象本身
        public static V Set<V>(this Dictionary<string, Variable> dict, string key, V value) where V : class
        {
            if (dict.TryGetValue(key, out Variable v))
            {
                return ((Variable<V>)v).value = value;
            }
            Variable<V> box = new Variable<V>(value);
            dict[key] = box;
            return box.value;
        }

        #endregion

        #region Get

        // 值类型：ref 返回，读写都直达字典里存的那份；键不存在会抛异常
        public static ref V GetRef<V>(this Dictionary<string, Variable> dict, string key) where V : unmanaged
        {
            if (dict.TryGetValue(key, out Variable v))
            {
                return ref ((Variable<V>)v).value;
            }
            throw new KeyNotFoundException($"变量字典里没有这个键：{key}");
        }

        public static V Get<V>(this Dictionary<string, Variable> dict, string key) where V : class
        {
            if (dict.TryGetValue(key, out Variable v) && v is Variable<V> box)
                return box.value;
            return null;
        }

        public static void Get<V>(this Dictionary<string, Variable> dict, string key, out V result) where V : class
        {
            if (dict.TryGetValue(key, out Variable v) && v is Variable<V> box)
                result = box.value;
            else
                result = null;
        }

        #endregion

        public static void Del(this Dictionary<string, Variable> dict, string key)
        {
            dict.Remove(key);
        }
    }
}
