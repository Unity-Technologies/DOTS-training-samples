using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlatformSpawnerSystem))]
public class PassengerSpawnerSystem : SystemBase
{
    public int frameCounter = 0;
    protected override void OnUpdate()
    {
        if(frameCounter < 1)
        {
            ++frameCounter;
            return;
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in PassengerSpawner spawner, in LocalToWorld spawnerLocalToWorld) =>
        {
            ecb.DestroyEntity(entity);

            float3 spawnerForwardVec = spawnerLocalToWorld.Forward;
            float3 spawnerRightVec = spawnerLocalToWorld.Right;

            float currSpacing = 0.0f;
            
            float3 queuePos = spawnerLocalToWorld.Position;

            for (int qIdx=0; qIdx < spawner.numQueues; ++qIdx)
            {
                float3 queueFrontPos = queuePos + spawnerRightVec * currSpacing;

                for (int pIdx=0; pIdx < spawner.passengersPerQueue; ++pIdx)
                {
                    var passengerEnt = ecb.Instantiate(spawner.passengerPrefab);
                    float3 pos = queueFrontPos - spawnerForwardVec * pIdx * spawner.passengerSpacing;
                    ecb.SetComponent(passengerEnt, new Translation() { Value = pos });
                }

                currSpacing += spawner.queueSpacing;
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
