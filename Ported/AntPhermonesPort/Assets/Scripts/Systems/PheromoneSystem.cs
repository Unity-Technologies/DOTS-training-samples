using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public partial class PheromoneSystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnStartRunning()
    {
        //var mapEntity = GetSingletonEntity<MapSetting>();
        //var mapSetting = GetComponent<MapSetting>(mapEntity);

        //if (texture != null && texture.width == mapSetting.Size && texture.height == mapSetting.Size)
        //    return;

        //if (texture != null)
        //    UnityEngine.GameObject.Destroy(texture);

        var config = GetSingleton<Config>();

        texture = new UnityEngine.Texture2D(
            (int)config.CellMapResolution, 
            (int)config.CellMapResolution, 
            UnityEngine.TextureFormat.RGBAFloat, 
            false
        );

        texture.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
        texture.filterMode = UnityEngine.FilterMode.Point;

        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();

        PheromoneMapHelper helper = new PheromoneMapHelper(EntityManager.GetBuffer<PheromoneMap>(pheromoneMapEntity), config.CellMapResolution, config.WorldSize);
        helper.InitPheromoneMap();

        var pheromoneMap = EntityManager
            .GetBuffer<PheromoneMap>(pheromoneMapEntity)
            .Reinterpret<float4>();

        texture.LoadRawTextureData(pheromoneMap.AsNativeArray());
        texture.Apply();

        //var pheromoneMapEntity = GetSingletonEntity<CellMap>();
        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);

        //renderMesh.material = new UnityEngine.Material(renderMesh.material);
        renderMesh.material.mainTexture = texture;
    }

    protected override void OnUpdate()
    {
        // In the update
        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();

        var pheromoneMap = EntityManager
            .GetBuffer<PheromoneMap>(pheromoneMapEntity)
            .Reinterpret<float4>();

        texture.LoadRawTextureData(pheromoneMap.AsNativeArray());
        texture.Apply();

        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);
        renderMesh.material.mainTexture = texture;
    }
}
