#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
#endif
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECFSM
{
#if !ODIN_INSPECTOR
    [Serializable]
    public class StringObjectDictionary : SerializableDictionary<string, Object> { }
#endif
#if ODIN_INSPECTOR
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
        LayerMask,
        Attribute,
        LItems,
        ClipTransitionAsset
    }
    public class Com : SerializedMonoBehaviour
#else
    public class Com : MonoBehaviour
#endif
    {
        // ʵ��
        public Entity entity;
        // ״̬���ݣ�����һ�������data���õ�״̬�б�
        public Data data;
        // ��ǰ״̬���
        public int stateno;

        // Unity����ע�루����������Ա����л���ʾ���༭���
#if ODIN_INSPECTOR
        public Dictionary<string, Object> injectObject = new Dictionary<string, Object>();
#else
        public StringObjectDictionary injectObject = new StringObjectDictionary();
#endif
        // ��ʼ��������������޷������л���ʾ���༭���ֻ����Odin��Odin��ô��ôǿ������
        public Dictionary<string, object> persistentData = new Dictionary<string, object>();
        // �����ֵ�
        [HideInInspector]
        public Dictionary<string, object> variables = new Dictionary<string, object>();

        void Awake()
        {
            variables.Add("_com", this);
            variables.Add("_entity", entity);
            foreach (var obj in injectObject)
                variables.Add(obj.Key, obj.Value);
        }

        public void OnUpdate()
        {
            data.states[stateno]?.onUpdate?.Invoke(variables);
        }

        public void OnFixedUpdate()
        {
            data.states[stateno]?.onFixedUpdate?.Invoke(variables);
        }

        public void ChangeState(int index)
        {
            data.states[stateno]?.onExit?.Invoke(variables);
            stateno = index;
            data.states[stateno]?.onEnter?.Invoke(variables);
        }

#if ODIN_INSPECTOR
        [SerializeField]
        private readonly string VKey;
        [InlineButton("AddV", "Add")]
        [SerializeField]
        private readonly ValueType VType;

        private void AddV()
        {
            object v = VType switch
            {
                ValueType.Int => 0,
                ValueType.Float => 0f,
                ValueType.Bool => false,
                ValueType.String => VKey,
                ValueType.Vector2 => Vector2.zero,
                ValueType.Vector2Int => Vector2Int.zero,
                ValueType.Vector3 => Vector3.zero,
                ValueType.Vector3Int => Vector3Int.zero,
                ValueType.Quaternion => Quaternion.identity,
                ValueType.LayerMask => default(LayerMask),
                _ => null
            };

            persistentData.Add(VKey, v);
        }
#endif
    }

}
