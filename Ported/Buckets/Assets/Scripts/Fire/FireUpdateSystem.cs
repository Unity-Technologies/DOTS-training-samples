using System;
using System.Data.SqlTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Water;
using Random = Unity.Mathematics.Random;

namespace Fire
{
    [GenerateAuthoringComponent]
    public struct FireSpreadProperties : IComponentData
    {
        public float SpreadFlashPoint;
    }

    struct FireData
    {
        Entity entity;
        TemperatureComponent temp;
    }

    public class FireUpdateSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;

        protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            // Check if we have initialized
            EntityQuery queryGroup = GetEntityQuery(typeof(Initialized));
            if (queryGroup.CalculateEntityCount() == 0)
            {
                return;
            }

            float deltaTime = Time.DeltaTime;
            float epsilon = Mathf.Epsilon;
            float elapsedTime = (float)Time.ElapsedTime;

            var fireBufferEntity = GetSingletonEntity<FireBuffer>();
            var gridBufferLookup = GetBufferFromEntity<FireBufferElement>();
            var gridBuffer = gridBufferLookup[fireBufferEntity];

            var gridArray = gridBuffer.AsNativeArray();
            var gridMetaData = EntityManager.GetComponentData<FireBufferMetaData>(fireBufferEntity);

            var addFireArr = new NativeArray<float>(gridArray.Length, Allocator.TempJob);

            // Create cached native array of temperature components to read neighbor temperature data in jobs
            NativeArray<TemperatureComponent> temperatureComponents = new NativeArray<TemperatureComponent>(gridArray.Length, Allocator.TempJob);
            for (int i = 0; i < gridArray.Length; i++)
            {
                temperatureComponents[i] = EntityManager.GetComponentData<TemperatureComponent>(gridArray[i].FireEntity);
                addFireArr[i] = 0f;
            }

            var ecb = m_ECBSystem.CreateCommandBuffer();
            float diminishAmount = .2f; //How much the neighboring fires distinguish less
            Entities
                .ForEach((Entity entity, ref TemperatureComponent temperature, ref ExtinguishAmount extinguishAmount) =>
                {
                    temperature.Value -= 1f;//extinguishAmount.Value;

                    var neighbors = FireSearch.GetNeighboringIndicies(temperature.GridIndex, gridMetaData.CountX, gridMetaData.CountZ);

                    float newExtinguishValue = extinguishAmount.Value - diminishAmount;

                    if (neighbors.Top != -1)
                    {
                        addFireArr[neighbors.Top] = newExtinguishValue;
                    }
                    if (neighbors.Bottom != -1)
                    {
                        addFireArr[neighbors.Bottom] = newExtinguishValue;
                    }
                    if (neighbors.Left != -1)
                    {
                        addFireArr[neighbors.Left] = newExtinguishValue;
                    }
                    if (neighbors.Right != -1)
                    {
                        addFireArr[neighbors.Right] = newExtinguishValue;
                    }
                    ecb.RemoveComponent<ExtinguishAmount>(entity);

                }).Run();

            float frameLerp = deltaTime * 8f;

            var ecbPar = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

            // Update fires in scene
            Entities
                .WithDeallocateOnJobCompletion(addFireArr)
                .WithDeallocateOnJobCompletion(temperatureComponents)
                .WithNone<ExtinguishAmount>()
                .ForEach((Entity fireEntity, int entityInQueryIndex, ref Translation position, ref TemperatureComponent temperature, ref FireColor color,
                    in BoundsComponent bounds, in StartHeight startHeight, in FireColorPalette pallete) =>
                {
                    if (addFireArr[temperature.GridIndex] > 0f)
                    {
                        ecbPar.AddComponent(entityInQueryIndex, fireEntity, new ExtinguishAmount{Value = addFireArr[temperature.GridIndex] });
                    }

                    var temp = math.clamp(temperature.Value, 0, 1);

                    // If temp is 0, velocity is put out
                    bool fireOut = UnityMathUtils.Approximately(temp, 0f, epsilon);
                    if (fireOut)
                    {
                        temperature.Velocity = 0;

                        // Find neighboring temperatures
                        var neighbors = FireSearch.GetNeighboringIndicies(temperature.GridIndex, gridMetaData.CountX, gridMetaData.CountZ);

                        var topTemp = (neighbors.Top == -1) ? 0 : temperatureComponents[neighbors.Top].Value;
                        var bottomTemp = (neighbors.Bottom == -1) ? 0 : temperatureComponents[neighbors.Bottom].Value;
                        var leftTemp = (neighbors.Left == -1) ? 0 : temperatureComponents[neighbors.Left].Value;
                        var rightTemp = (neighbors.Right == -1) ? 0 : temperatureComponents[neighbors.Right].Value;

                        // Find max temp of neighbors
                        var maxTemp = math.max(topTemp, bottomTemp);
                        maxTemp = math.max(maxTemp, leftTemp);
                        maxTemp = math.max(maxTemp, rightTemp);

                        if (maxTemp >= (0.95f - temperature.IgnitionVariance))
                        {
                            temperature.Velocity = temperature.StartVelocity * (1 + temperature.IgnitionVariance);
                        }
                    }

                    // Update temp with velocity
                    var deltaVel = temperature.Velocity * deltaTime;
                    temperature.Value = math.clamp(temp + deltaVel, 0, 1);

                    // Compute variance for fire height fluctation
                    float fireVariance = math.sin(5 * temperature.Value * elapsedTime + 100 * (1 + temperature.IgnitionVariance)) * startHeight.Variance * temperature.Value;

                    // Compute new height
                    float newHeight = bounds.SizeY / 2f + fireVariance;
                    float heightFrameTarget = startHeight.Value + newHeight * temperature.Value;
                    position.Value.y = math.lerp(position.Value.y, heightFrameTarget, frameLerp);

                    // Update fire color
                    float4 newColor = fireOut ? pallete.UnlitColor : UnityMathUtils.Lerp(pallete.LitLowColor, pallete.LitHighColor, temperature.Value);
                    color.Value = UnityMathUtils.Lerp(color.Value, newColor, frameLerp);
                }).ScheduleParallel();

            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
