using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

/// <summary>
/// This file is copy and modified from
/// "com.unity.visualscripting/Runtime/VirtualScripting.Core/Unity/Singleton.cs"
/// </summary>
namespace E
{
    public static class Singleton<T> where T : MonoBehaviour, ISingleton
    {
        private static readonly SingletonAttribute m_Attribute;

        private static readonly object m_Lock = new object();

        private static readonly HashSet<T> m_Awoken;

        private static T m_Instance;

        static Singleton()
        {
            m_Awoken = new HashSet<T>();
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

        public static bool Instantiated
        {
            get
            {
                if (Application.isPlaying)
                {
                    return m_Instance != null;
                }
                else
                {
                    lock (m_Lock)
                    {
                        return FindInstances().Length == 1;
                    }
                }
            }
        }

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
                                Instantiate();
                            }
                        }
                    }
                    return m_Instance;
                }
                else
                {
                    lock (m_Lock)
                    {
                        return Instantiate();
                    }
                }
            }
        }

        private static T[] FindInstances()
        {
            // Fails here on hidden hide flags
            return UnityObject.FindObjectsOfType<T>();
        }

        private static T Instantiate()
        {
            T[] instances = FindInstances();
            if (instances.Length == 1)
            {
                m_Instance = instances[0];
            }
            else if (instances.Length == 0)
            {
                if (m_Attribute.Automatic)
                {
                    // Create the parent game object with the proper hide flags
                    GameObject singleton = new GameObject(m_Attribute.Name ?? typeof(T).Name);
                    singleton.hideFlags = m_Attribute.HideFlags;

                    // Instantiate the component, letting Awake assign the real instance variable
                    T _instance = singleton.AddComponent<T>();
                    _instance.hideFlags = m_Attribute.HideFlags;

                    // Sometimes in the editor, for example when creating a new scene,
                    // AddComponent seems to call Awake add a later frame, making this call
                    // fail for exactly one frame. We'll force-awake it if need be.
                    Awake(_instance);

                    // Make the singleton persistent if need be
                    if (m_Attribute.Persistent && Application.isPlaying)
                    {
                        UnityObject.DontDestroyOnLoad(singleton);
                    }
                }
                else
                {
                    throw new UnityException($"Missing '{typeof(T)}' singleton in the scene.");
                }
            }
            else if (instances.Length > 1)
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
            }
            return m_Instance;
        }

        public static void Awake(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), "Value cannot be null.");
            }
            if (m_Awoken.Contains(instance))
            {
                return;
            }
            if (m_Instance != null)
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
            }
            m_Instance = instance;
            m_Awoken.Add(instance);
        }

        public static void OnDestroy(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), "Value cannot be null.");
            }
            if (m_Instance == instance)
            {
                m_Instance = null;
            }
            else
            {
                throw new UnityException($"Trying to destroy invalid instance of '{typeof(T)}' singleton.");
            }
        }
    }
}