using System.Text.RegularExpressions;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    partial struct CarLaneSystem : ISystem
    {
        ComponentDataFromEntity<Translation> m_TranslationDataFromEntity;

        public void OnCreate(ref SystemState state)
        {
            m_TranslationDataFromEntity = state.GetComponentDataFromEntity<Translation>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_TranslationDataFromEntity.Update(ref state);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var carTransformAspect in SystemAPI.Query<TransformAspect>().WithAll<Car>())
            {
                
            }
            
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<CarDynamicBuffer>>())
            {
                var entities = buffer.AsNativeArray().Reinterpret<Entity>();
                
                // Sort entities
                for (int i = 0; i < entities.Length; i++)
                {
                    var translation = m_TranslationDataFromEntity[entities[i]];
                    translation.Value.y += 0.2f * state.Time.DeltaTime;
                    ecb.SetComponent(entities[i], translation);
                }
            }
        }
    }
}
