using UnityEngine;

namespace E
{
    [ExecuteInEditMode]
    public class TestState : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnUpdate()
        {

        }

        public class StateMachine
        {
            public enum State
            {
                None = 0,      // -> Awaked
                Awaked = 1,    // -> Enabled | Destroyed
                Enabled = 2,   // -> OnUpdate | Disabled
                OnUpdate = 3,  // -> Disabled
                Disabled = 4,  // -> Enabled | Destroyed
                Destroyed = 5
            }

            private State m_State = State.None;

            private bool m_IsAlive = false;

            private bool m_IsActived = false;

            public void InternalAwake()
            {
                //if(!allowCreate) return;
                m_IsAlive = true;
                if (m_State == State.None)
                {
                    //OnAwake();
                    m_State = State.Awaked;
                }
            }

            private void InternalEnable()
            {
                switch (m_State)
                {
                    case State.Awaked:
                    case State.Disabled:
                        if (m_IsAlive && m_IsActived)
                        {
                            //OnEnable();
                            m_State = State.Enabled;
                        }
                        break;
                }
            }

            private void InternalUpdate()
            {
                switch (m_State)
                {
                    case State.Enabled:
                    case State.OnUpdate:
                        if (m_IsAlive && m_IsActived)
                        {
                            //OnUpdate();
                            m_State = State.OnUpdate;
                        }
                        break;
                }
            }

            private void InternalDisable()
            {
                switch (m_State)
                {
                    case State.Enabled:
                    case State.OnUpdate:
                        if (!m_IsAlive || !m_IsActived)
                        {
                            //OnDisable();
                            m_State = State.Disabled;
                        }
                        break;
                }
            }

            public void InternalDestroy()
            {
                m_IsAlive = false;
                switch (m_State)
                {
                    case State.Awaked:
                    case State.Disabled:
                        //OnDestroy();
                        m_State = State.Destroyed;
                        break;
                    case State.Enabled:
                    case State.OnUpdate:
                        //OnDisable();
                        //OnDestroy();
                        m_State = State.Destroyed;
                        break;
                }
            }

        }
    }
}