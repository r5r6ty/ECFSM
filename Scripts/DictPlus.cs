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
        public static void* GetPtr<K, V>(this Dictionary<K, V> dict, K key, out ulong gcHandler)
        {
            gcHandler = 0;
            V value = dict[key];
            if (value == null) return null;
            if (value.GetType().IsValueType)
            {
                byte* valuePtr = (byte*) UnsafeUtility.PinGCObjectAndGetAddress(value, out ulong handler);
                gcHandler = handler;
                byte* dataPtr = valuePtr + ClassHeader.HeaderSize;
                return dataPtr;
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
                byte* valuePtr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(value, out ulong handler);
                gcHandler = handler;
                byte* dataPtr = valuePtr + ClassHeader.HeaderSize;
                return dataPtr;
            }
            else
            {
                return null;
            }    
        }

        public static void* Set<V>(this Dictionary<string, Variable> dict, string key, V value) where V : struct
        {
            Variable<V> v = new Variable<V>(value);
            if (dict.TryAdd(key, v))
                return v.ptr;
            else
                return null;
        }

        public static V Set<K, V>(this Dictionary<K, Variable> dict, K key, V value) where V : class
        {
            Variable<V> v = new Variable<V>(value);
            if (dict.TryAdd(key, v))
                return v.value;
            else
                return null;
        }

        public static T Get<T>(this Dictionary<string, Variable> dict, string key) where T : class
        {
            if (dict.TryGetValue(key, out Variable value))
                return UnsafeUtility.As<Variable, Variable<T>>(ref value).value;
            //return ((Variable<T>)value).value;
            else
                return null;
        }

        public static void* Get(this Dictionary<string, Variable> dict, string key)
        {
            if (dict.TryGetValue(key, out Variable value))
                return value.ptr;
            else
                return null;
        }

        public static void Del()
        {

        }
    }
}
