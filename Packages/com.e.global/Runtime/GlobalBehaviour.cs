using System;

namespace E
{
    public abstract partial class GlobalBehaviour : IDisposable
    {
        private bool m_LastActive;

        private bool m_CurrActive;

        internal void Check(out bool enableState, out bool updateState, out bool disableState)
        {
            m_LastActive = m_CurrActive;
            updateState = m_CurrActive = IsActive();
            enableState = !m_LastActive && m_CurrActive;
            disableState = m_LastActive && !m_CurrActive;
        }

        // Add/初始化时立即执行
        internal void ExecuteAwake()
        {
            m_LastActive = m_CurrActive = false;
            Awake();
        }

        internal void ExecuteEnable()
        {
            OnEnable();
        }

        internal void ExecuteUpdate()
        {
            Update();
        }

        internal void ExecuteDisable()
        {
            OnDisable();
        }

        internal void CheckExecuteDisable()
        {
            if (m_LastActive) OnDisable();
        }

        protected abstract bool IsActive();

        protected abstract void Awake();

        protected virtual void OnEnable() { }

        protected virtual void Update() { }

        protected virtual void OnDisable() { }

        #region Dispose

        private bool m_DisposedValue;

        protected void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
                    m_LastActive = m_CurrActive = false;
                    DisposeManaged();
                }
                DisposeUnmanaged();
                //立刻从数据中移除
                m_DisposedValue = true;
            }
        }

        protected abstract void DisposeManaged();

        protected abstract void DisposeUnmanaged();

        ~GlobalBehaviour()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}