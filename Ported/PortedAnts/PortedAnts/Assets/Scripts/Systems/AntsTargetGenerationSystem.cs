using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct AntsTargetGenerationSystem : ISystem
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

            Entity home = state.EntityManager.Instantiate(config.AntsTargetPrefab);
            var homePosition = new float2(1f, 1f) * config.MapSize * .5f;

            state.EntityManager.SetComponentData(home,
                LocalTransform.FromPositionRotationScale(
                    new float3(homePosition.x, 0f, homePosition.y),
                    quaternion.identity,
                    config.ObstacleRadius * 2f));
            state.EntityManager.SetComponentData(home, new AntsTarget()
            {
                radius = config.ObstacleRadius,
                position = homePosition,
                isHome = true
            });
            state.EntityManager.AddComponentData(home, new URPMaterialPropertyBaseColor { Value = config.HomeColor });


            Entity food = state.EntityManager.Instantiate(config.AntsTargetPrefab);
            float foodAngle = rand.NextFloat() * 2f * math.PI;
            var foodPosition = new float2(1f, 1f) * config.MapSize * .5f + new float2(math.cos(foodAngle) * config.MapSize * .475f, math.sin(foodAngle) * config.MapSize * .475f);

            state.EntityManager.SetComponentData(food,
                LocalTransform.FromPositionRotationScale(
                    new float3(foodPosition.x, 0f, foodPosition.y),
                    quaternion.identity,
                    config.ObstacleRadius * 2f));
            state.EntityManager.SetComponentData(food, new AntsTarget()
            {
                radius = config.ObstacleRadius,
                position = foodPosition,
                isHome = false
            });
            state.EntityManager.AddComponentData(food, new URPMaterialPropertyBaseColor { Value = config.FoodColor });
        }
    }
}