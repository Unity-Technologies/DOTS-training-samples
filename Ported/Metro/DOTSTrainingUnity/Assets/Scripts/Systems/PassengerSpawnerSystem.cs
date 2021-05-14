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
        var random = new Random(1337);
        var friendRandom = new Random(1338);

        Entities.ForEach((Entity entity, in PassengerSpawner spawner, in LocalToWorld spawnerLocalToWorld) =>
        {
            ecb.DestroyEntity(entity);

            float3 spawnerForwardVec = spawnerLocalToWorld.Forward;
            float3 spawnerRightVec = spawnerLocalToWorld.Right;
            quaternion spawnerRot = spawnerLocalToWorld.Rotation;

            float currSpacing = 0.0f;
            
            float3 queuePos = spawnerLocalToWorld.Position;

            for (int qIdx=0; qIdx < spawner.numQueues; ++qIdx)
            {
                float3 queueFrontPos = queuePos + spawnerRightVec * currSpacing;

                for (int pIdx=0; pIdx < spawner.passengersPerQueue; ++pIdx)
                {
                    quaternion randRot = math.mul(quaternion.RotateY(math.lerp(-0.1f, 0.1f, random.NextFloat())), spawnerRot);
                    float randScaleY = math.lerp(0.6f, 1.0f, random.NextFloat());
                    float randScaleXZ = math.lerp(0.6f, 1.0f, random.NextFloat());
                    float offsetMax = 0.1f;
                    float randOffsetX = math.lerp(-offsetMax, offsetMax, random.NextFloat());
                    float randOffsetY = math.lerp(-offsetMax, offsetMax, random.NextFloat());
                    float randOffsetZ = math.lerp(-offsetMax, offsetMax, random.NextFloat());

                    var passengerEnt = ecb.Instantiate(spawner.passengerPrefab);
                    float3 pos = queueFrontPos - spawnerForwardVec * pIdx * spawner.passengerSpacing + new float3(randOffsetX, randOffsetY, randOffsetZ);
                    ecb.SetComponent(passengerEnt, new Translation() { Value = pos });
                    ecb.SetComponent(passengerEnt, new Rotation() { Value = randRot });
                    ecb.AddComponent(passengerEnt, new NonUniformScale() { Value = new float3(randScaleXZ, randScaleY, randScaleXZ) });
                }

                // Friends
                for (int pIdx = 0; pIdx < spawner.passengersPerQueue; ++pIdx)
                {
                    if(friendRandom.NextFloat() > 0.4f)
                    {
                        continue;
                    }

                    float friendDistance = math.lerp(0.5f, 0.7f, random.NextFloat());
                    if (random.NextFloat() > 0.1f)
                    {
                        friendDistance = -friendDistance;
                    }

                    float3 friendOffset = spawnerRightVec * friendDistance;

                    quaternion randRot = math.mul(quaternion.RotateY(math.lerp(-0.1f, 0.1f, random.NextFloat())), spawnerRot);
                    float randScaleY = math.lerp(0.6f, 1.0f, random.NextFloat());
                    float randScaleXZ = math.lerp(0.6f, 1.0f, random.NextFloat());
                    float offsetMax = 0.1f;
                    float randOffsetX = math.lerp(-offsetMax, offsetMax, random.NextFloat());
                    float randOffsetY = math.lerp(-offsetMax, offsetMax, random.NextFloat());
                    float randOffsetZ = math.lerp(-offsetMax, offsetMax, random.NextFloat());

                    var passengerEnt = ecb.Instantiate(spawner.passengerPrefab);
                    float3 pos = queueFrontPos - spawnerForwardVec * pIdx * spawner.passengerSpacing + new float3(randOffsetX, randOffsetY, randOffsetZ) + friendOffset;
                    ecb.SetComponent(passengerEnt, new Translation() { Value = pos });
                    ecb.SetComponent(passengerEnt, new Rotation() { Value = randRot });
                    ecb.AddComponent(passengerEnt, new NonUniformScale() { Value = new float3(randScaleXZ, randScaleY, randScaleXZ) });
                }

                currSpacing += spawner.queueSpacing;
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
