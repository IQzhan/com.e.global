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

        private static T m_Instance;

        public static T Instance
        { get { return CreateInstance(); } }

        public static T CreateInstance()
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
            return m_Instance;
        }

        public static bool Exists
        {
            get
            {
                if (Application.isPlaying)
                {
                    return m_Instance != null;
                }
                else
                {
                    if (m_Instance == null)
                    {
                        lock (m_Lock)
                        {
                            T[] objs = FindObjectsOfType<T>();
                            if (objs.Length == 1)
                            {
                                m_Instance = objs[0];
                            }
                        }
                    }
                    return m_Instance != null;
                }
            }
        }

        public static void DestroyInstance()
        {
            T[] objs = FindObjectsOfType<T>();
            for (int i = 0; i < objs.Length; i++)
            {
                SafeDestroy(objs[i]);
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
            }
            else if (objs.Length == 1)
            {
                m_Instance = objs[0];
            }
            else
            {
                // How to find the correct one?
                throw new Exception($"Instance of '{InstanceName}' can not be more then one.");
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
                    if (Utility.AllowLog)
                    {
                        Utility.Log($"{InstanceName} Loaded in {PlatformMode}.");
                    }
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

        public static string InstanceName
        {
            get
            {
                if (m_Attribute != null &&
                    !string.IsNullOrWhiteSpace(m_Attribute.Name))
                {
                    return m_Attribute.Name;
                }
                return typeof(T).Name;
            }
        }

        private string PlatformMode
        {
            get
            {
                return Application.isPlaying ? "playing mode" : "editor mode";
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
            SafeDestroyImmediate(instance.gameObject);
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