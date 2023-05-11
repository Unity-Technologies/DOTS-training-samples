using Miscellaneous.Execute;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

// [UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct WaterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<OmnibotSpawner>();
        state.RequireForUpdate<Grid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Grid>();

        var startPoint = config.GridOrigin -
                         new float3(4, 0, 4);
        var endPoint = new float3(startPoint.x + config.GridSize * 1.05f, 0, startPoint.z + config.GridSize * 1.05f) +
                       new float3(5, 0, 5);

        var point1 = startPoint;
        var point2 = new float3(startPoint.x, 0, endPoint.z);
        var point3 = endPoint;
        var point4 = new float3(endPoint.x, 0, startPoint.z);
        
        state.EntityManager.Instantiate(config.WaterPrefab, (int)(config.GridSize / 2), Allocator.Temp);
        foreach (var (trans, water, ptMatrix) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Water>, RefRW<PostTransformMatrix>>())
        {
            float3 spawnPoint = default;
            int sideID = Random.Range(0, 4);
            if (sideID == 0)
            {
                spawnPoint = math.lerp(point1, point2, Random.Range(0f, 1f));
            }
            else if (sideID == 1)
            {
                spawnPoint = math.lerp(point2, point3, Random.Range(0f, 1f));
            }
            else if (sideID == 2)
            {
                spawnPoint = math.lerp(point3, point4, Random.Range(0f, 1f));
            }
            else if (sideID == 3)
            {
                spawnPoint = math.lerp(point4, point1, Random.Range(0f, 1f));
            }

            trans.ValueRW.Position = spawnPoint;
            water.ValueRW.TargetScale = new float2(Random.Range(2f, 5f), Random.Range(2f, 5f));
        }
    }
}