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
        // Unity����ע�루����������Ա����л���ʾ���༭���
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
        public Dictionary<int , State> states = new Dictionary<int , State>();
    }
}