using Unity.Entities;
using Unity.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(WallSpawnerSystem))]
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

        //texture = new UnityEngine.Texture2D(500, 500, UnityEngine.TextureFormat.RGBAFloat, false);
        //texture.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
        //texture.filterMode = UnityEngine.FilterMode.Point;

        //for(int x = 0; x < texture.width/2; x++)
        //{
        //    for(int y = 0; y<texture.height/2; ++y)
        //    {
        //        texture.SetPixel(x, y, new UnityEngine.Color(10000, 0, 0, 1));
        //    }
        //}

        //var buffer = GetBuffer<Pheromone>(mapEntity).Reinterpret<float4>();

        //texture.LoadRawTextureData(buffer.AsNativeArray());
        //texture.Apply();

        //var pheromoneMapEntity = GetSingletonEntity<CellMap>();
        //var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);

        //renderMesh.material = new UnityEngine.Material(renderMesh.material);
        //renderMesh.material.mainTexture = texture;
    }

    protected override void OnUpdate()
    {
        // In the update
        //var pheromoneMapEntity = GetSingletonEntity<CellMap>();
        //var pheromoneMapBuffer = GetBuffer<Pheromone>(pheromoneMapEntity).Reinterpret<float4>();

        //texture.LoadRawTextureData(pheromoneMapBuffer.AsNativeArray());
        //texture.Apply();
    }
}
