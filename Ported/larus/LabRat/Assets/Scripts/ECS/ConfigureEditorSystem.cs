using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Scenes;

#if UNITY_EDITOR
[ExecuteAlways]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysSynchronizeSystem]
public class ConfigureEditorSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            return;
        // Editor World
        World.GetOrCreateSystem<SceneSystem>().BuildSettingsGUID = ConfigureClientSystems.ClientBuildSettingsGUID;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
    }
}
#endif
