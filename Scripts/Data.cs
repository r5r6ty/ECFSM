using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECFSM
{
    public class State
    {
        public Action<Dictionary<string, object>> onEnter;
        public Action<Dictionary<string, object>> onFixedUpdate;
        public Action<Dictionary<string, object>> onUpdate;
        public Action<Dictionary<string, object>> onExit;
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