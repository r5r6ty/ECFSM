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
        public Action<Dictionary<string, Variable>> onEnter;
        public Action<Dictionary<string, Variable>> onFixedUpdate;
        public Action<Dictionary<string, Variable>> onUpdate;
        public Action<Dictionary<string, Variable>> onExit;
    }


    public class Data : SerializedScriptableObject
    {
        // Unity物体注入（依靠插件可以被序列化显示到编辑器里）
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
        public Dictionary<int , State> states = new Dictionary<int , State>();
    }
}