using ECFSM;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DictPlus {
    public class ClassHeader {
        public byte value = 0;
        public static int HeaderSize {
            get { return UnsafeUtility.GetFieldOffset(typeof(ClassHeader).GetField(nameof(value))); }
        }
    }
    public static unsafe class DictPlus {
        public static void* GetPtr<K>(this Dictionary<K, Variable> dict, K key, out ulong gcHandler)
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
            Variable<V> v = new Variable<V>(value);
            if (dict.TryAdd(key, v))
                return v.value;
            else
                return null;
        }

        public static void* Set<V>(this Dictionary<string, Variable> dict, string key, V value) where V : unmanaged
        {
            Variable<V> v = new Variable<V>(value);
            if (dict.TryAdd(key, v))
                return v.ptr;
            else
                return null;
        }
        #endregion

        #region Get
        public static T Get<T>(this Dictionary<string, Variable> dict, string key) where T : class
        {
            if (dict.TryGetValue(key, out Variable value))
                return UnsafeUtility.As<Variable, Variable<T>>(ref value).value;
            //return ((Variable<T>)value).value;
            else
                return null;
        }


        public static void Get<T>(this Dictionary<string, Variable> dict, string key, out T result) where T : class
        {
            if (dict.TryGetValue(key, out Variable value))
                result = UnsafeUtility.As<Variable, Variable<T>>(ref value).value;
            //return ((Variable<T>)value).value;
            else
                result = null;
        }

        public static void* Get(this Dictionary<string, Variable> dict, string key)
        {
            if (dict.TryGetValue(key, out Variable value))
                return value.ptr;
            else
                return null;
        }

        // 相同参数用不了，除非改函数名
        //public static T* Get<T>(this Dictionary<string, Variable> dict, string key) where T : unmanaged
        //{
        //    if (dict.TryGetValue(key, out Variable value))
        //        return (T*)value.ptr;
        //    else
        //        return null;
        //}

        public static void Get<T>(this Dictionary<string, Variable> dict, string key, out T* result) where T : unmanaged
        {
            if (dict.TryGetValue(key, out Variable value))
                result = (T*)value.ptr;
            else
                result = null;
        }
        #endregion

        public static void Del(this Dictionary<string, Variable> dict, string key)
        {
            if(dict.Remove(key, out Variable value))
                if (value.gcHandler > 0)
                    UnsafeUtility.ReleaseGCObject(value.gcHandler);
        }
    }
}
