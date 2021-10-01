using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

// Render Pheromone Map

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class PheromoneSystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnStartRunning()
    {
        var config = GetSingleton<Config>();

        texture = new UnityEngine.Texture2D(
            (int)config.CellMapResolution,
            (int)config.CellMapResolution,
            UnityEngine.TextureFormat.RFloat,
            false
        );

        texture.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
        texture.filterMode = UnityEngine.FilterMode.Point;

        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();

        var pheromoneMap = EntityManager
            .GetBuffer<PheromoneMap>(pheromoneMapEntity)
            .Reinterpret<float>();

        texture.LoadRawTextureData(pheromoneMap.AsNativeArray());
        texture.Apply();

        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);
        renderMesh.material.mainTexture = texture;
    }

    protected override void OnUpdate()
    {
        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();

        var pheromoneMap = EntityManager
            .GetBuffer<PheromoneMap>(pheromoneMapEntity)
            .Reinterpret<float>();

        texture.LoadRawTextureData(pheromoneMap.AsNativeArray());
        texture.Apply();

        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);
        renderMesh.material.mainTexture = texture;
    }
}
