using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct TrainSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (trainSpawn, entity) in SystemAPI.Query<TrainSpawn>().WithEntityAccess())
            {
                var carriages = CollectionHelper.CreateNativeArray<Entity>(trainSpawn.CarriageCount, Allocator.Temp);
                ecb.Instantiate(trainSpawn.CarriageSpawn, carriages);

                /*var train = SystemAPI.GetComponent<Train>(entity);
                var trainTransform = SystemAPI.GetComponent<LocalTransform>(entity);*/

                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriageEntity = carriages[i];
                    var carriage = new Carriage
                    {
                        Index = i,
                        Train = entity,
                        /*TrainComponent = train,
                        TrainTransform = trainTransform,*/
                    };
                    ecb.AddComponent(carriageEntity, carriage);
                }

                var spawnerEntity = trainSpawn.CarriageSpawn;
                ecb.RemoveComponent<TrainSpawn>(entity);
                ecb.DestroyEntity(spawnerEntity);
                carriages.Dispose();
            }

            state.Enabled = false;
        }
    }
}