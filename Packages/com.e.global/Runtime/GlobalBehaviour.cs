using UnityEngine;

namespace E
{
    public abstract partial class GlobalBehaviour
    {
        public void Init(in int id, in bool isExecuteInEditorMode)
        {
            ID = id;
            IsExecuteInEditorMode = isExecuteInEditorMode;
        }

        public int ID { get; private set; }

        public bool IsExecuteInEditorMode { get; private set; }

        public bool IsAlive { get => Application.isPlaying || IsExecuteInEditorMode; }

        public bool IsAwaked { get; private set; }

        internal bool IsLastActived { get; private set; }

        public bool IsActived { get => IsAwaked && IsEnabled; }

        protected abstract bool IsEnabled { get; }

        protected virtual void OnAwake() { }

        protected virtual void OnEnable() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDestroy() { }

        internal void InernalAwake()
        {
            if (IsAlive && !IsAwaked)
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
                }
                OnDestroy();
            }
        }
    }
}