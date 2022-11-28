using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct BeeSpawnSystem : ISystem
{
    EntityQuery baseColorQuery;
    private EntityQuery transformQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        transformQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var hive in SystemAPI.Query<Hive>())
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var bees = CollectionHelper.CreateNativeArray<Entity>(hive.startBeeCount, Allocator.Temp);
            ecb.Instantiate(hive.beePrefab, bees);
            
            var colorQueryMask = baseColorQuery.GetEntityQueryMask();
            var transformQueryMask = transformQuery.GetEntityQueryMask();

            var color = new URPMaterialPropertyBaseColor { Value = (Vector4)hive.color };
            foreach (var bee in bees)
            {
                ecb.SetComponentForLinkedEntityGroup(bee, colorQueryMask, color);
                var pos = hive.boundsPosition;
                pos.y = bee.Index;

                var noiseValue = noise.cnoise(pos / 10f);
                var position = hive.boundsPosition + noiseValue * 2f * hive.boundsExtents;
                position.y = hive.boundsPosition.y - hive.boundsExtents.y;
                var trans = WorldTransform.FromPosition(position);
                ecb.SetComponentForLinkedEntityGroup(bee, transformQueryMask, new LocalTransform()
                {
                    Position = position,
                    Scale = 1f
                });
            }
        }

        state.Enabled = false;
    }
}