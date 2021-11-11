using UnityEngine;

namespace E
{
    public abstract partial class GlobalBehaviour
    {
        public int ID { get; internal set; } = -1;

        public bool IsExecuteInEditorMode { get; internal set; } = false;

        public bool IsAlive { get => Application.isPlaying || IsExecuteInEditorMode; }

        public bool IsAwaked { get; private set; } = false;

        internal bool IsLastActived { get; private set; } = false;

        public bool IsActived { get => IsAwaked && IsEnabled; }

        protected abstract bool IsEnabled { get; }

        protected virtual void OnAwake() { }

        protected virtual void OnEnable() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDestroy() { }

        internal void InernalAwake()
        {
            if (IsAlive && !IsAwaked && !IsLastActived)
            {
                OnAwake();
                IsAwaked = true;
            }
        }

        internal void InernalEnable()
        {
            if (!IsLastActived && IsActived)
            {
                OnEnable();
                IsLastActived = true;
            }
        }

        internal void InternalUpdate()
        {
            if (IsActived)
            {
                OnUpdate();
                IsLastActived = true;
            }
        }

        internal void InternalDisable()
        {
            if (IsLastActived && !IsActived)
            {
                OnDisable();
                IsLastActived = false;
            }
        }

        internal void InternalDestroy()
        {
            if (IsAwaked)
            {
                if (IsLastActived)
                {
                    OnDisable();
                    IsLastActived = false;
                }
                OnDestroy();
                IsAwaked = false;
            }
        }
    }
}