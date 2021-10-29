using System;
using UnityEngine;

namespace E
{
    public abstract class GlobalBehaviour<T> : GlobalBehaviour
        where T : GlobalBehaviour<T>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Load()
        {
            
        }
    }

    public abstract partial class GlobalBehaviour : IDisposable
    {
        private bool m_LastActive;

        private bool m_CurrActive;

        internal void Setup()
        {
            m_LastActive = m_CurrActive = false;
            Initialize();
        }

        internal void Check(out bool enableState, out bool updateState, out bool disableState)
        {
            m_LastActive = m_CurrActive;
            updateState = m_CurrActive = IsActive();
            enableState = !m_LastActive && m_CurrActive;
            disableState = m_LastActive && !m_CurrActive;
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

        internal void ExecuteDrawGizmos(bool selected)
        {
            OnDrawGizmos(selected);
        }

        protected abstract void Initialize();

        protected abstract bool IsActive();

        protected virtual void OnEnable() { }

        protected virtual void Update() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDrawGizmos(bool selected) { }

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