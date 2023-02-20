#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
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

#if ODIN_INSPECTOR
    public class Data : SerializedScriptableObject
#else
    public class Data : ScriptableObject
#endif
    {
        // Unity����ע�루����������Ա����л���ʾ���༭���
#if ODIN_INSPECTOR
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
#else
        public StringObjectDictionary injectObject = new StringObjectDictionary();
#endif
        public Dictionary<int , State> states = new Dictionary<int , State>();
    }
}