using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECFSM
{
    public class State
    {
        public Action<MiniDict<string>> onEnter;
        public Action<MiniDict<string>> onFixedUpdate;
        public Action<MiniDict<string>> onUpdate;
        public Action<MiniDict<string>> onExit;
    }


    public class Data : SerializedScriptableObject
    {
        // Unity物体注入（依靠插件可以被序列化显示到编辑器里）
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
        public Dictionary<int , State> states = new Dictionary<int , State>();
    }
}