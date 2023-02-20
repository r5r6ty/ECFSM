#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
#endif
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECFSM
{
    public abstract class Variable { }

    public class Variable<T> : Variable
    {
        public T value;
        unsafe public void* ptr;
        public Variable(T value)
        {
            this.value = value;
        }
    }
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
        public Dictionary<string, object> persistentVariables = new Dictionary<string, object>();

        // �����ֵ�
        //[HideInInspector]
        private MiniDict<string> variables = new MiniDict<string>();

        void Awake()
        {
            unsafe
            {
                // ����һЩ�������ı���
                variables.Set("_com", this);
                variables.Set("_entity", entity);
                // ����ע����Ϸ����
                foreach (var obj in injectObject)
                {
                    Object c = obj.Value;
                    variables.Set(obj.Key, c);
                }
                // ����ע��ֵ����
                foreach (var v in persistentVariables)
                {
                    // ½��׷��
                    switch (v.Value.GetType())
                    {
                        case Type t when t == typeof(int):
                            {
                                int c = (int)v.Value;
                                variables.Set(v.Key, c);
                            }
                            break;
                        case Type t when t == typeof(float):
                            {
                                float c = (float)v.Value;
                                variables.Set(v.Key, c);
                            }
                            break;
                        case Type t when t == typeof(bool):
                            {
                                bool c = (bool)v.Value;
                                variables.Set(v.Key, c);
                            }
                            break;
                        case Type t when t == typeof(Vector3):
                            {
                                Vector3 c = (Vector3)v.Value;
                                variables.Set(v.Key, c);
                            }
                            break;
                        case Type t when t == typeof(LayerMask):
                            {
                                LayerMask c = (LayerMask)v.Value;
                                variables.Set(v.Key, c);
                            }
                            break;
                        default:
                            Debug.LogWarning("д��δע��������ͣ�");
                            variables.Set(v.Key, v);
                            break;
                    }
                }

                #region ���Բ��Բ��Բ��Բ��Բ��Բ��Բ���
                //print(variables.Get("_com"));
                //print(variables.Get("HP"));



                //ref object HP = ref variables.Get("HP");

                //int aasd = 256;
                //variables.Add("HP2", &aasd);
                //int* ggg = (int*)variables.Int("HP");

                //Debug.Log($"{*ggg},{*ggg}");
                //Debug.Log(*ggg);
                //Debug.Log(*ggg);
                //Debug.Log(*ggg);
                //*ggg += 1;
                //Debug.Log(*ggg);


                //object[] thy = new object[12];
                //thy[3] = (Int32)337845;

                ////��ȡ�ṹ��ռ�ÿռ�Ĵ�С
                //int nSize = Marshal.SizeOf(thy[3]);
                ////����һ����ͬ��С���ڴ�ռ�
                //IntPtr intPtr = Marshal.AllocHGlobal(nSize);
                ////Struct->IntPtr
                //Marshal.StructureToPtr(thy[3], intPtr, true);
                ////IntPtr->Struct
                ////Int32 Info = (Int32)Marshal.PtrToStructure(intPtr, typeof(Int32));

                //void* ppttt = intPtr.ToPointer();
                //int* sd2ss = (int*)ppttt;
                //Debug.Log(*sd2ss);
                //*sd2ss = 119119;
                //Debug.Log(thy[3]);



                //object[] sdd = new object[12];
                //sdd[3] = 3375;
                //UnsafeTool unsafeTool2 = new UnsafeTool();
                //byte* ppt = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(sdd, 3).ToPointer();
                //void* ptpt = *(void**)ppt;
                //int sd2 = (int)unsafeTool2.voidPtrToObject(ptpt);
                //Debug.Log(sd2);
                //IntPtr ipt = Marshal.UnsafeAddrOfPinnedArrayElement(sdd, 3);
                //int d = UnsafeUtility.As<IntPtr, int>(ref *(IntPtr*)ipt);
                //Debug.Log(d);

                //UnsafeTool unsafeTool = new UnsafeTool();
                //int vvv = 123456;
                //byte* objPtr = (byte*)&vvv;

                //int* sd = (int*)objPtr;
                //Debug.Log(*sd);
                #endregion
            }
        }

        public void OnStart()
        {
            if (data.states.TryGetValue(stateno, out State state))
            {
                state?.onEnter?.Invoke(variables);
            }
        }

        public void OnUpdate()
        {
            if (data.states.TryGetValue(stateno, out State state))
            {
                state?.onUpdate?.Invoke(variables);
            }
        }

        public void OnFixedUpdate()
        {
            if (data.states.TryGetValue(stateno, out State state))
            {
                state?.onFixedUpdate?.Invoke(variables);
            }
        }

        public void ChangeState(int index)
        {
            if (data.states.TryGetValue(stateno, out State state))
            {
                state?.onExit?.Invoke(variables);
            }
            stateno = index;
            if (data.states.TryGetValue(stateno, out state))
            {
                state?.onEnter?.Invoke(variables);
            }
        }

#if ODIN_INSPECTOR
        // ׷��ֵ�����õ�
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

            persistentVariables.Add(VKey, v);
        }
#endif
    }
}
