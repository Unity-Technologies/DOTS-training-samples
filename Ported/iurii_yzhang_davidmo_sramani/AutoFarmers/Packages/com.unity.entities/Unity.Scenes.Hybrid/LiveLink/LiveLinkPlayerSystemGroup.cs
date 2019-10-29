using System.IO;
using Unity.Entities;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    
    //@TODO: #ifdefs massively increase iteration time right now when building players (Should be fixed in 20.1)
    //       Until then always have the live link code present.
#if UNITY_EDITOR
    [DisableAutoCreation]
#endif
    [ExecuteAlways]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SceneSystem))]
    class LiveLinkRuntimeSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            Enabled = File.Exists(SceneSystem.GetBootStrapPath());
        }
    }
}