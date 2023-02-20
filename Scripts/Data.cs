using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Object = UnityEngine.Object;

namespace ECFSM
{
    public class State
    {
        public Action<MiniDict2<string>> onEnter;
        public Action<MiniDict2<string>> onFixedUpdate;
        public Action<MiniDict2<string>> onUpdate;
        public Action<MiniDict2<string>> onExit;
    }

#if ODIN_INSPECTOR
    public class Data : SerializedScriptableObject
#else
    public class Data : ScriptableObject
#endif
    {
        // Unity物体注入（依靠插件可以被序列化显示到编辑器里）
#if ODIN_INSPECTOR
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
#else
        public StringObjectDictionary injectObject = new StringObjectDictionary();
#endif
        public Dictionary<int , State> states = new Dictionary<int , State>();
    }
}