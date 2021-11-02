using System;
using UnityEngine;

namespace E
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Singleton<T>
    {
        static Singleton()
        {
            object[] attrs = typeof(T).GetCustomAttributes(typeof(SingletonAttribute), true);
            if (attrs.Length > 0)
            {
                m_Attribute = attrs[0] as SingletonAttribute;
            }
            if (m_Attribute == null)
            {
                throw new TypeAccessException($"Missing singleton attribute for '{typeof(T)}'.");
            }
        }

        private static readonly SingletonAttribute m_Attribute;

        private static readonly object m_Lock = new object();

        [SerializeField, HideInInspector]
        private GameObject m_CreatedGameobject;

        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (Application.isPlaying)
                {
                    if (m_Instance == null)
                    {
                        lock (m_Lock)
                        {
                            if (m_Instance == null)
                            {
                                FindInstance();
                            }
                        }
                    }
                }
                else
                {
                    lock (m_Lock)
                    {
                        FindInstance();
                    }
                }
                return m_Instance;
            }
        }

        private void Awake()
        {
            DoAwake();
            Create();
        }

        private void OnDestroy()
        {
            Destroy();
            DoDestroy();
        }

        protected virtual void Create() { }

        protected virtual void Destroy() { }

        private static void FindInstance()
        {
            T[] objs = FindObjectsOfType<T>();
            if (objs.Length == 0)
            {
                GameObject obj = new GameObject(InstanceName);
                m_Instance = obj.AddComponent<T>();
                m_Instance.m_CreatedGameobject = obj;
            }
            else if (objs.Length == 1)
            {
                m_Instance = objs[0];
                m_Instance.m_CreatedGameobject = null;
            }
            else
            {
                throw new System.Exception("Instance of " + InstanceName + " can not be more then one.");
            }
        }

        private void DoAwake()
        {
            lock (m_Lock)
            {
                T thisOne = this as T;
                if (m_Instance != null)
                {
                    if (m_Instance != thisOne)
                    {
                        SafeDestroy(thisOne);
                        return;
                    }
                }
                else
                {
                    T[] objs = FindObjectsOfType<T>();
                    if (objs.Length > 1)
                    {
                        SafeDestroy(thisOne);
                        return;
                    }
                    m_Instance = thisOne;
                    KeepAlive(thisOne);
                    hideFlags = m_Attribute.HideFlags;
                    Debug.Log(InstanceName + " " + Platform + " Loaded");
                }
            }
        }

        private void DoDestroy()
        {
            lock (m_Lock)
            {
                T thisOne = this as T;
                if (m_Instance == thisOne)
                {
                    m_Instance = null;
                }
            }
        }

        private static string InstanceName
        {
            get { return m_Attribute.Name; }
        }

        private string Platform
        {
            get
            {
                return Application.isPlaying ? "Playing" : "Editor";
            }
        }

        private void KeepAlive(T instance)
        {
            if (Application.isPlaying && m_Attribute.Persistent)
            {
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        private static void SafeDestroy(T instance)
        {
            if (instance.m_CreatedGameobject != null)
            {
                SafeDestroyImmediate(instance.m_CreatedGameobject);
            }
            else
            {
                SafeDestroyImmediate(instance);
            }
        }

        private static void SafeDestroyImmediate<T0>(T0 obj) where T0 : UnityEngine.Object
        {
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }
    }
}