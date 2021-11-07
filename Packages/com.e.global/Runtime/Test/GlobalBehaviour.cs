using UnityEngine;

namespace E
{
    public abstract partial class GlobalBehaviour_0 : ScriptableObject
    {
        private bool m_Initialized;

        private bool m_LastActive;

        private bool m_CurrActive;

        internal bool actualDestroy;

        [SerializeField]
        private bool m_ExecuteInEditor;

        public bool ExecuteInEditor
        { get { return m_ExecuteInEditor; } }

        public bool Initialized
        { get { return m_Initialized; } }

        internal void Check(
            out bool awakeState,
            out bool enableState,
            out bool updateState,
            out bool disableState)
        {
            m_LastActive = m_CurrActive;
            awakeState = !m_Initialized;
            updateState = m_CurrActive = Actived;
            enableState = !m_LastActive && m_CurrActive;
            disableState = m_LastActive && !m_CurrActive;
        }

        internal void ExecuteAwake()
        {
            if (!m_Initialized)
            {
                m_LastActive = m_CurrActive = false;
                AwakeCallback();
                m_Initialized = true;
            }
        }

        internal void ExecuteEnable()
        {
            if (m_Initialized)
            {
                EnableCallback();
            }
        }

        internal void ExecuteUpdate()
        {
            if (m_Initialized)
            {
                UpdateCallback();
            }
        }

        internal void ExecuteDisable()
        {
            if (m_Initialized)
            {
                DisableCallback();
            }
        }

        internal void ExecuteDestroy()
        {
            if (m_Initialized)
            {
                if (m_LastActive) DisableCallback();
                m_Initialized = false;
                DestroyCallback();
                m_LastActive = m_CurrActive = false;
                if (actualDestroy)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(this);
                    }
                    else
                    {
                        DestroyImmediate(this);
                    }
                }
            }
        }

        public bool Actived
        {
            get
            {
                return
                    ((!Application.isPlaying && m_ExecuteInEditor) ||
                    Application.isPlaying) &&
                    m_Initialized && IsActive();
            }
        }

        protected abstract bool IsActive();

        protected abstract void AwakeCallback();

        protected virtual void EnableCallback() { }

        protected virtual void UpdateCallback() { }

        protected virtual void DisableCallback() { }

        protected abstract void DestroyCallback();
    }
}