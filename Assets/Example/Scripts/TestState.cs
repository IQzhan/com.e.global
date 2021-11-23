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

        private class StateMachine
        {
            private enum State
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

            public void CallCreate()
            {
                //if(!allowCreate) return;
                m_IsAlive = true;
                ExecuteState();
            }

            public void CallDestroy()
            {
                m_IsAlive = false;
                ExecuteState();
            }

            private void CallUpdateState()
            {
                ExecuteState();
            }

            private void ExecuteState()
            {
                //系统生命周期OnEnable OnUpdate OnDisable
                //判断系统当前生命周期,已经到了或过了就直接执行，没到就入列
                //仅限OnEnable OnUpdate OnDisable
                State lastState = m_State;
                switch (m_State)
                {
                    case State.None:
                        if (m_IsAlive)
                        {
                            //OnAwake();
                            m_State = State.Awaked;
                        }
                        break;
                    case State.Awaked:
                        if (m_IsAlive && m_IsActived)
                        {
                            //OnEnable();
                            m_State = State.Enabled;
                        }
                        else if (!m_IsAlive)
                        {
                            //OnDestroy();
                            m_State = State.Destroyed;
                        }
                        break;
                    case State.Enabled:
                        if (m_IsAlive && m_IsActived)
                        {
                            //OnUpdate();
                            m_State = State.OnUpdate;
                        }
                        else if (!m_IsAlive || !m_IsActived)
                        {
                            //OnDisable();
                            m_State = State.Disabled;
                        }
                        break;
                    case State.OnUpdate:
                        if (!m_IsAlive || !m_IsActived)
                        {
                            //OnDisable();
                            m_State = State.Disabled;
                        }
                        //OnUpdate();
                        if (!m_IsAlive || !m_IsActived)
                        {
                            //OnDisable();
                            m_State = State.Disabled;
                        }
                        break;
                    case State.Disabled:
                        if (m_IsAlive && m_IsActived)
                        {
                            //OnEnable();
                            m_State = State.Enabled;
                        }
                        else if (!m_IsAlive)
                        {
                            //OnDestroy();
                            m_State = State.Destroyed;
                        }
                        break;
                    case State.Destroyed:
                        break;
                }
                if (lastState != m_State) ExecuteState();
            }

        }
    }
}