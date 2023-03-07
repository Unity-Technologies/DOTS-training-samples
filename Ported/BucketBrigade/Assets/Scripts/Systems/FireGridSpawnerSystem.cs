using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Color = Components.Color;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct FireGridSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
            var buffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
            buffer.Length = config.numRows * config.numColumns;
            
            var index = 0;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < config.numRows; i++)
            {
                for (int j = 0; j < config.numColumns; j++)
                {
                    var flameCellEntity = ecb.Instantiate(config.flameCellPrefab);
                    //var flameCellEntity = state.EntityManager.Instantiate(config.flameCellPrefab);
                    //state.EntityManager.SetComponentData(fireCell, new FireCell{ cellIndex = index});
                    ecb.AddComponent(flameCellEntity, new URPMaterialPropertyBaseColor {Value = config.fireNeutralColor});
                    //state.EntityManager.AddComponentData(flameCellEntity, new URPMaterialPropertyBaseColor {Value = config.colourFireCellNeutral});
                    //buffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
                    buffer[index] = new ConfigAuthoring.FlameHeat{ Value = 0f};
                    index++;
                }
            }
            
            ecb.Playback(state.EntityManager);

            var row = 0;
            var col = 0;
            foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<FlameCell>())
            {
                transform.ValueRW = LocalTransform.FromPosition(row * config.cellSize, -0.49f, col * config.cellSize);
                if (++col >= config.numColumns)
                {
                    row++;
                    col = 0;
                }
            }
            
            var rand = new Random( 123);
            buffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
            for (int i = 0; i < config.startingFireCount; i++)
            {
                var randIndex = rand.NextInt(buffer.Length);
                buffer[randIndex] = new ConfigAuthoring.FlameHeat {Value = 1f};
            }

        }

    }
}
