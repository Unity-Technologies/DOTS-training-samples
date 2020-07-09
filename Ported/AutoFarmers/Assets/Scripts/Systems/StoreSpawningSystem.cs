using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GridInitializationSystem))]
    public class StoreSpawningSystem : SystemBase
    {
        private EntityCommandBufferSystem m_CommandBufferSystem;
        private Random m_Random;

        // Set on spawners to make them go to sleep after init
        public struct StoresAreInitalizedTag : IComponentData
        {
        };

        protected override void OnCreate()
        {
            m_Random = new Random(0x1234567);
            m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        static bool IsAreaFree(DynamicBuffer<CellTypeElement> buffer, int2 gridSize, int2 position, int minDist)
        {
            for (int j = -minDist; j < minDist; j++)
            {
                for (int i = -minDist; i < minDist; i++)
                {
                    int idx = (position.x + i) + (position.y + j) * gridSize.x;
                    if (buffer[idx].Value != CellType.Raw)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void OnUpdate()
        {
            var ecb = m_CommandBufferSystem.CreateCommandBuffer();
            var random = m_Random;

            var entity = GetSingletonEntity<Grid>();
            var typeBuffer = EntityManager.GetBuffer<CellTypeElement>(entity);
            var entityBuffer = EntityManager.GetBuffer<CellEntityElement>(entity);
            int2 gridSize = GetSingleton<Grid>().Size;

            Entities
            .WithNone<StoresAreInitalizedTag>()
            .ForEach((Entity storeSpawner, in StoreSpawner spawner) =>
            {
                int minDist = (spawner.MinDistance == 0) ? 1 : spawner.MinDistance;
                int2 minDist2 = new int2(minDist, minDist);

                for (int n = 0; n < spawner.NumStores; n++)
                {
                    int2 position = new int2();
  
                    for (int tries =  0; tries < 100; tries++)
                    {
                        position = random.NextInt2(minDist2, gridSize - minDist2);

                        if ( IsAreaFree(typeBuffer, gridSize, position, minDist))
                        {
                            break;
                        }
                    }

                    // Box position account for the position being the minimum but the model we use having the pivot at the center
                    var translation = new float3(position.x, 0.0f, position.y);
                    translation += new float3(0.5f,0.5f, 0.5f);

                    // Inset the box
                    //scale.x -= 0.2f;
                   // scale.z -= 0.2f;

                    var instance = ecb.Instantiate(spawner.StorePrefab);
                    ecb.SetComponent(instance, new Translation { Value = translation });

                    int idx = position.x  + position.y  * gridSize.x;
                    typeBuffer[idx] = new CellTypeElement { Value = CellType.Shop };
                }
                ecb.AddComponent(storeSpawner, new StoresAreInitalizedTag());
            }).Run();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}