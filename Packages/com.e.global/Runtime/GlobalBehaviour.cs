using System;
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
                try
                {
                    OnAwake();
                    IsAwaked = true;
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        internal void InernalEnable()
        {
            if (!IsLastActived && IsActived)
            {
                try
                {
                    OnEnable();
                    IsLastActived = true;
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        internal void InternalUpdate()
        {
            if (IsActived)
            {
                try
                {
                    OnUpdate();
                    IsLastActived = true;
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        internal void InternalDisable()
        {
            if (IsLastActived && !IsActived)
            {
                try
                {
                    OnDisable();
                    IsLastActived = false;
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        internal void InternalDestroy()
        {
            if (IsAwaked)
            {
                if (IsLastActived)
                {
                    try
                    {
                        OnDisable();
                        IsLastActived = false;
                    }
                    catch (Exception e)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                try
                {
                    OnDestroy();
                    IsAwaked = false;
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}