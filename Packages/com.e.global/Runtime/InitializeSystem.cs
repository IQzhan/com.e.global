#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    /// <summary>
    /// Control when to initialize or destroy global system.
    /// </summary>
    internal static class InitializeSystem
    {
#if UNITY_EDITOR
        // Execute these editor methods by <1> <2> <3> <4> <5> order.

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadInEditor()
        {
            // <1> Execute when
            //       <open the editor>
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update -= CheckUpdater;
            EditorApplication.update += CheckUpdater;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void DidReloadScripts()
        {
            // <2> Execute when
            //       <open the editor>                  
            //       <enter play mode from editor mode> 
            //       <enter editor mode from play mode> 
            //       <reload assemblies>
            DestroyOnExit();
            InitializeOnLoad();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            // <3> Execute when
            //       <enter editor mode from play mode> 
            //       <enter play mode from editor mode> 
            //       <exit editor mode>                 
            //       <exit play mode>                   
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    InitializeOnLoad();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    DestroyOnExit();
                    break;
            }
        }

        [UnityEditor.Callbacks.PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // <4> Execute when
            //      <after building the player>
            DestroyOnExit();
            InitializeOnLoad();
        }

        private static void CheckUpdater()
        {
            // <5> Execute when
            //       <Update>
            GlobalUpdater.CreateInstance();
        }
#else
        // Execute at runtime after builded

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoadAtRuntime()
        {
            InitializeOnLoad();
            UnityEngine.Application.quitting -= DestroyOnExit;
            UnityEngine.Application.quitting += DestroyOnExit;
        }
#endif

        /// <summary>
        /// Call on initialize game.
        /// </summary>
        private static void InitializeOnLoad()
        {
            GlobalTime.InitializeOnLoad();
            BehaviourManager.InitializeOnLoad();
            GlobalUpdater.CreateInstance();
        }

        /// <summary>
        /// Call on exit game.
        /// </summary>
        private static void DestroyOnExit()
        {
            BehaviourManager.DestroyOnExit();
            GlobalTime.DestroyOnExit();
        }
    }
}