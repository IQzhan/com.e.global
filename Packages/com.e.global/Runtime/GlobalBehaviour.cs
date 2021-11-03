using UnityEngine;

namespace E
{
    public abstract partial class GlobalBehaviour : ScriptableObject
    {
        private bool m_Initialized;

        private bool m_LastActive;

        private bool m_CurrActive;

        [SerializeField]
        private bool m_ExecuteInEditor;

        public bool ExecuteInEditor
        { get { return m_ExecuteInEditor; } }

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
            m_LastActive = m_CurrActive = false;
            AwakeCallback();
            m_Initialized = true;
        }

        internal void ExecuteEnable()
        {
            EnableCallback();
        }

        internal void ExecuteUpdate()
        {
            UpdateCallback();
        }

        internal void ExecuteDisable()
        {
            DisableCallback();
        }

        internal void CheckExecuteDisable()
        {
            if (m_LastActive) DisableCallback();
        }

        internal void ExecuteDestroy()
        {
            m_Initialized = false;
            DestroyCallback();
            m_LastActive = m_CurrActive = false;
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