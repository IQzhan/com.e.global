using System;

namespace E
{
    public abstract partial class GlobalBehaviour : IDisposable
    {
        protected abstract void Initialize();

        public abstract bool IsActive();

        public virtual void OnEnable() { }

        public virtual void Update() { }

        public virtual void OnDisable() { }

        public virtual void OnDrawGizmos(bool selected) { }

        #region Dispose

        private bool m_DisposedValue;

        protected void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
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