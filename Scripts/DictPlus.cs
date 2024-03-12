using ECFSM;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace DictPlus {
    public class ClassHeader {
        public byte value = 0;
        public static int HeaderSize {
            get { return UnsafeUtility.GetFieldOffset(typeof(ClassHeader).GetField(nameof(value))); }
        }
    }
    public static unsafe class DictPlus {
        public static void* GetPtr(this Dictionary<string, Variable> dict, string key, out ulong gcHandler)
        {
            gcHandler = 0;
            if (dict.TryGetValue(key, out Variable value))
            {
                if (value.GetType().IsValueType)
                {
                    byte* valuePtr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(value, out gcHandler);
                    byte* dataPtr = valuePtr + ClassHeader.HeaderSize;
                    return dataPtr;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static void* GetPtr<V>(ref V value, out ulong gcHandler)
        {
            gcHandler = 0;
            if (value == null) return null;
            if (value.GetType().IsValueType)
            {
                byte* valuePtr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(value, out gcHandler);
                byte* dataPtr = valuePtr + ClassHeader.HeaderSize;
                return dataPtr;
            }
            else
            {
                return null;
            }    
        }

        #region Set
        public static V Set<K, V>(this Dictionary<K, Variable> dict, K key, V value) where V : class
        {
            if (dict.TryGetValue(key, out var v))
            {
                ((Variable<V>)v).value = value;
                return value;
            }
            else
            {
                v = new Variable<V>(value);
                dict.TryAdd(key, v);
                return ((Variable<V>)v).value;
            }
        }

        public static V* Set<V>(this Dictionary<string, Variable> dict, string key, V value) where V : unmanaged
        {
            if (dict.TryGetValue(key, out var v))
            {
                V* ptr = (V*)v.ptr;
                *ptr = value;
                return ptr;
            }
            else
            {
                v = new Variable<V>(value);
                dict.TryAdd(key, v);
                return (V*)v.ptr;
            }
        }
        #endregion

        #region Get
        public static V Get<V>(this Dictionary<string, Variable> dict, string key) where V : class
        {
            if (dict.TryGetValue(key, out Variable value))
                return UnsafeUtility.As<Variable, Variable<V>>(ref value).value;
            //return ((Variable<V>)value).value;
            else
                return null;
        }

        public static void Get<V>(this Dictionary<string, Variable> dict, string key, out V result) where V : class
        {
            if (dict.TryGetValue(key, out Variable value))
                result = UnsafeUtility.As<Variable, Variable<V>>(ref value).value;
            //return ((Variable<V>)value).value;
            else
                result = null;
        }

        // CS0111: 类型“DictPlus”已定义了一个名为“Get”的具有相同参数类型的成员
        //public static V* Get<V>(this Dictionary<string, Variable> dict, string key) where V : unmanaged
        //{
        //    if (dict.TryGetValue(key, out Variable value))
        //        return (V*)value.ptr;
        //    else
        //        return null;
        //}

        public static void* Get(this Dictionary<string, Variable> dict, string key)
        {
            if (dict.TryGetValue(key, out Variable value))
                return value.ptr;
            else
                return null;
        }

        public static void Get<V>(this Dictionary<string, Variable> dict, string key, out V* result) where V : unmanaged
        {
            if (dict.TryGetValue(key, out Variable value))
                result = (V*)value.ptr;
            else
                result = null;
        }
        #endregion

        public static void Del(this Dictionary<string, Variable> dict, string key)
        {
            if (dict.Remove(key, out Variable value))
                value.Release();
        }
    }
}
