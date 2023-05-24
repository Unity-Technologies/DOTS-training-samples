using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct ObstacleGenerationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<Config>();

            var rand = Random.CreateFromIndex(1);

            for (int i = 1; i <= config.ObstacleRingCount; i++)
            {
                float ringRadius = (i / (config.ObstacleRingCount + 1f)) * (config.MapSize * .5f);
                float circumference = ringRadius * 2f * math.PI;
                int maxCount = config.ObstacleRadius == 0f ? 0 :  (int)math.ceil(circumference / config.ObstacleRadius);
                int offset = rand.NextInt(0, maxCount);
                int holeCount = rand.NextInt(1, 3);
                for (int j = 0; j < maxCount; j++)
                {
                    float t = (float)j / maxCount;
                    if ((t * holeCount) % 1f < config.ObstaclesFillRate)
                    {
                        float angle = (j + offset) / (float)maxCount * (2f * math.PI);

                        var position = new float2(
                            config.MapSize * .5f + math.cos(angle) * ringRadius,
                            config.MapSize * .5f + math.sin(angle) * ringRadius);

                        Entity newEntity = state.EntityManager.Instantiate(config.ObstaclePrefab);
                        state.EntityManager.SetComponentData(newEntity,
                            LocalTransform.FromPositionRotationScale(
                                new float3(position.x, 0f, position.y),
                                quaternion.identity,
                                config.ObstacleRadius * 2));
                        state.EntityManager.SetComponentData(newEntity, new Obstacle() { position = position, radius = config.ObstacleRadius });
                    }
                }
            }
        }
    }
}
