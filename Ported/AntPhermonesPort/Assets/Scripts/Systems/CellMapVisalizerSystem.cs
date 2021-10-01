using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;


public partial class CellMapVisualizerSystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnStartRunning()
    {
        return;

        var config = GetSingleton<Config>();

        //if (!config.DisplayCellMap) return;

        texture = new UnityEngine.Texture2D(
            (int)config.CellMapResolution,
            (int)config.CellMapResolution,
            UnityEngine.TextureFormat.RGBAFloat,
            false
        );

        texture.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
        texture.filterMode = UnityEngine.FilterMode.Point;

        Entity cellMapEntity = GetSingletonEntity<CellMap>();

        CellMapHelper helper = new CellMapHelper(EntityManager.GetBuffer<CellMap>(cellMapEntity), config.CellMapResolution, config.WorldSize);
        helper.InitCellMap();

        var cellMap = EntityManager
            .GetBuffer<CellMap>(cellMapEntity);

        NativeArray<float4> arr = new NativeArray<float4>(cellMap.Length, Allocator.Temp);
        for (int i = 0; i < cellMap.Length; ++i)
        {
            switch (cellMap[i].state)
            {
                case CellState.Empty:
                    arr[i] = new float4(0, 0, 0, 1f);
                    break;
                case CellState.IsNest:
                    arr[i] = new float4(0, 0, 1, 1);
                    break;
                case CellState.IsFood:
                    arr[i] = new float4(0, 1, 0, 1);
                    break;
                case CellState.IsObstacle:
                    arr[i] = new float4(0.8f, 0.8f, 0.8f, 1);
                    break;
                case CellState.HasLineOfSightToFood:
                    arr[i] = new float4(1, 1, 0, 1);
                    break;
                case CellState.HasLineOfSightToNest:
                    arr[i] = new float4(0, 1, 1, 1);
                    break;
                case CellState.HasLineOfSightToBoth:
                    arr[i] = new float4(1, 1, 1, 1);
                    break;
            }
        }

        texture.LoadRawTextureData(arr);
        texture.Apply();

        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(cellMapEntity);

        renderMesh.material.mainTexture = texture;
    }

    protected override void OnUpdate()
    {
        
    }
}
