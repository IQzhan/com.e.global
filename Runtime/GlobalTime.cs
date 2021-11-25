namespace E
{
    /// <summary>
    /// Global system time information base on <see cref="GlobalSettings.Method"/> and <see cref="GlobalSettings.DeltaTime"/>.
    /// </summary>
    public class GlobalTime
    {
        #region Public properties

        /// <summary>
        /// true if initialized.
        /// </summary>
        public static bool IsReady => instance.m_IsReady;

        /// <summary>
        /// The time in seconds at the beginning of this frame
        /// based on  <see cref="GlobalSettings.Method"/> and <see cref="GlobalSettings.DeltaTime"/>
        /// since the global system initialized.
        /// </summary>
        public static double Time => instance.m_Time;

        /// <summary>
        /// The interval in seconds from the last frame to the current one
        /// based on  <see cref="GlobalSettings.Method"/> and <see cref="GlobalSettings.DeltaTime"/>
        /// since the global system initialized.
        /// </summary>
        public static double DeltaTime => instance.m_DeltaTime;

        /// <summary>
        /// The real time in seconds right now
        /// since the global system initialized.
        /// </summary>
        public static double RealTime => instance.m_IsReady ? instance.m_Stopwatch.Elapsed.TotalSeconds : 0;

        /// <summary>
        /// The real time in milliseconds right now
        /// since the global system initialized.
        /// </summary>
        public static long Milliseconds => instance.m_IsReady ? instance.m_Stopwatch.ElapsedMilliseconds : 0;

        /// <summary>
        /// The frame count
        /// based on  <see cref="GlobalSettings.Method"/> and <see cref="GlobalSettings.DeltaTime"/>
        /// since the global system initialized.
        /// </summary>
        public static ulong FrameCount => instance.m_IsReady ? instance.m_FrameCount : 0;

        #endregion

        #region Private properties

        /// <summary>
        /// This instance.
        /// </summary>
        internal static readonly GlobalTime instance;

        private bool m_IsReady;

        private System.Diagnostics.Stopwatch m_Stopwatch;

        private double m_Time, m_LastTime, m_DeltaTime;

        private ulong m_FrameCount;

        #endregion

        #region Initialize & Dispose

        static GlobalTime() => instance = new GlobalTime();

        private GlobalTime() { }

        ~GlobalTime() { Destroy(); }

        internal static void InitializeOnLoad() => instance.Initialize();

        internal static void DestroyOnExit() => instance.Destroy();

        private void Initialize()
        {
            if (m_IsReady) return;
            m_Time = m_LastTime = m_DeltaTime = 0;
            m_FrameCount = 0;
            m_Stopwatch = new System.Diagnostics.Stopwatch();
            m_Stopwatch.Start();
            m_IsReady = true;
        }

        private void Destroy()
        {
            if (!m_IsReady) return;
            if (m_Stopwatch != null)
            {
                m_Stopwatch.Stop();
                m_Stopwatch = null;
            }
            m_FrameCount = 0;
            m_Time = m_LastTime = m_DeltaTime = 0;
            m_IsReady = false;
        }

        #endregion

        #region Life Cycle methods

        internal bool FixedUpdate() => CallUpdate(GlobalSettings.UpdateMethod.FixedUpdate);

        internal bool Update() => CallUpdate(GlobalSettings.UpdateMethod.Update);

        internal bool LateUpdate() => CallUpdate(GlobalSettings.UpdateMethod.LateUpdate);

        private bool CallUpdate(GlobalSettings.UpdateMethod updateMethod)
        {
            if (m_IsReady && GlobalSettings.Method == updateMethod)
            {
                double realTime = m_Stopwatch.Elapsed.TotalSeconds;
                if (realTime - m_LastTime >= GlobalSettings.DeltaTime)
                {
                    m_Time = realTime;
                    m_DeltaTime = m_Time - m_LastTime;
                    m_LastTime = m_Time;
                    ++m_FrameCount;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}