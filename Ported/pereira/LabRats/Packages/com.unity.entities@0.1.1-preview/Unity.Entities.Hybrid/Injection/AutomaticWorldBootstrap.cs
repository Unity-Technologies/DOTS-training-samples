#if UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP
#define UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_RUNTIME_WORLD
#endif

using UnityEngine;

namespace Unity.Entities
{
#if !UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_RUNTIME_WORLD
    static class AutomaticWorldBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            DefaultWorldInitialization.Initialize("Default World", false);
        }
    }
#endif
}
