using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Water
{
    public class ExtinguishSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;

         protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            /*
            // Check if we have initialized
            EntityQuery queryGroup = GetEntityQuery(typeof(Fire.Initialized));
            if (queryGroup.CalculateEntityCount() == 0)
            {
                return;
            }
        
            // Grab Fire buffer
            var fireBufferEntity = GetSingletonEntity<Fire.FireBuffer>();
            var gridBufferLookup = GetBufferFromEntity<Fire.FireBufferElement>();
            var gridBuffer = gridBufferLookup[fireBufferEntity];
            var gridArray = gridBuffer.AsNativeArray();
        
            var gridMetaData = EntityManager.GetComponentData<Fire.FireBufferMetaData>(fireBufferEntity);

            var ecb = m_ECBSystem.CreateCommandBuffer();

            // Create cached native array of temperature components to read neighbor temperature data in jobs
            //NativeArray<Fire.TemperatureComponent> temperatureComponents = new NativeArray<Fire.TemperatureComponent>(gridArray.Length, Allocator.TempJob);
            //for (int i = 0; i < gridArray.Length; i++)
            //{
            //    temperatureComponents[i] = EntityManager.GetComponentData<Fire.TemperatureComponent>(gridArray[i].FireEntity);
            //}

            float diminishAmount = .2f; //How much the neighboring fires distinguish less
            Entities.ForEach((Entity entity, ref Fire.TemperatureComponent temperature, ref ExtinguishAmount extinguishAmount) => {
                if(extinguishAmount.Value <= 0)
                {
                    ecb.RemoveComponent<ExtinguishAmount>(entity);
                    return;
                }

                temperature.Value -= extinguishAmount.Value; 

                var neighbors = FireSearch.GetNeighboringIndicies(temperature.GridIndex, gridMetaData.CountX, gridMetaData.CountZ);

                float newExtinguishValue = extinguishAmount.Value - diminishAmount;

                if (neighbors.Top != -1)
                    ecb.AddComponent(gridArray[neighbors.Top].FireEntity, new ExtinguishAmount{ Value = newExtinguishValue });
                if (neighbors.Bottom != -1)
                    ecb.AddComponent(gridArray[neighbors.Bottom].FireEntity, new ExtinguishAmount{ Value = newExtinguishValue });
                if (neighbors.Left != -1)
                    ecb.AddComponent(gridArray[neighbors.Left].FireEntity, new ExtinguishAmount{ Value = newExtinguishValue });
                if (neighbors.Right != -1)
                    ecb.AddComponent(gridArray[neighbors.Right].FireEntity, new ExtinguishAmount{ Value = newExtinguishValue });
                
                ecb.RemoveComponent<ExtinguishAmount>(entity);

            }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
            */
        }
    }
}

