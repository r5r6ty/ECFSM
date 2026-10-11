using DictPlus;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECFSM
{
    // 初始变量下拉框里能选的类型
    public enum ValueType
    {
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector2Int,
        Vector3,
        Vector3Int,
        Quaternion,
        LayerMask
    }

    public class Com : SerializedMonoBehaviour
    {
        // 实体
        public Entity entity;
        // 状态数据（存有一个种类的data共用的状态列表）
        public Data data;
        // 当前状态编号
        public int stateno;

        // Unity物体注入（依靠插件可以被序列化显示到编辑器里）
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
        // 初始变量（<string, object>需要Odin才能序列化显示到编辑器里）
        public Dictionary<string, object> persistentVariables = new Dictionary<string, object>();

        // 变量字典（运行时填充，不序列化）
        [NonSerialized]
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        void Awake()
        {
            // 设置一些访问外界的变量
            variables.Set("_com", this);
            variables.Set("_entity", entity);
            variables.Set("_data", data);
            // 设置注入游戏物体
            foreach (var obj in injectObject)
            {
                variables.Set(obj.Key, obj.Value);
            }
            // 设置注入值变量
            foreach (var v in persistentVariables)
            {
                SetPersistent(v.Key, v.Value);
            }
        }

        private void OnDestroy()
        {
            variables?.Clear();
        }

        public void OnStart()
        {
            Current?.onEnter?.Invoke(variables);
        }

        public void OnUpdate()
        {
            Current?.onUpdate?.Invoke(variables);
        }

        public void OnFixedUpdate()
        {
            Current?.onFixedUpdate?.Invoke(variables);
        }

        public void ChangeState(int index)
        {
            Current?.onExit?.Invoke(variables);
            stateno = index;
            Current?.onEnter?.Invoke(variables);
        }

        private State Current
        {
            get { return data != null && data.states.TryGetValue(stateno, out State state) ? state : null; }
        }

        private void SetPersistent(string key, object value)
        {
            switch (value)
            {
                case int i: variables.SetRef(key, i); break;
                case float f: variables.SetRef(key, f); break;
                case bool b: variables.SetRef(key, b); break;
                case string s: variables.Set(key, s); break;
                case Vector2 v2: variables.SetRef(key, v2); break;
                case Vector2Int v2i: variables.SetRef(key, v2i); break;
                case Vector3 v3: variables.SetRef(key, v3); break;
                case Vector3Int v3i: variables.SetRef(key, v3i); break;
                case Quaternion q: variables.SetRef(key, q); break;
                case LayerMask lm: variables.SetRef(key, lm); break;
                default:
                    Debug.LogWarning($"写入未注册过的类型：{value?.GetType().Name ?? "null"}（键：{key}）", this);
                    break;
            }
        }

        // 追加值变量用的
        [InlineButton("AddV", "Add")]
        [SerializeField]
        private string VKey = "";
        [SerializeField]
        private ValueType VType = ValueType.Int;

        private void AddV()
        {
            if (string.IsNullOrEmpty(VKey))
            {
                Debug.LogWarning("变量键名不能为空", this);
                return;
            }

            object v = VType switch
            {
                ValueType.Int => 0,
                ValueType.Float => 0f,
                ValueType.Bool => false,
                ValueType.String => "",
                ValueType.Vector2 => Vector2.zero,
                ValueType.Vector2Int => Vector2Int.zero,
                ValueType.Vector3 => Vector3.zero,
                ValueType.Vector3Int => Vector3Int.zero,
                ValueType.Quaternion => Quaternion.identity,
                ValueType.LayerMask => default(LayerMask),
                _ => null
            };

            persistentVariables[VKey] = v;
        }
    }
}
