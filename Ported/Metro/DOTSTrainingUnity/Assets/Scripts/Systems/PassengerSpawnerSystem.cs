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

        Entities.ForEach((Entity entity, in PassengerSpawner spawner, in LocalToWorld spawnerLocalToWorld, in Rotation spawnerRot) =>
        {
            ecb.DestroyEntity(entity);

            float3 spawnerForwardVec = math.rotate(spawnerRot.Value, new float3(0.0f, 0.0f, 1.0f));
            float3 spawnerRightVec = math.rotate(spawnerRot.Value, new float3(1.0f, 0.0f, 0.0f));

            float currSpacing = 0.0f;
            
            float3 queuePos = spawnerLocalToWorld.Position;

            for (int qIdx=0; qIdx < spawner.numQueues; ++qIdx)
            {
                float3 queueFrontPos = queuePos - spawnerRightVec * currSpacing;

                for (int pIdx=0; pIdx < spawner.passengersPerQueue; ++pIdx)
                {
                    var passengerEnt = ecb.Instantiate(spawner.passengerPrefab);
                    float3 pos = queueFrontPos + spawnerForwardVec * pIdx * spawner.passengerSpacing;
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
