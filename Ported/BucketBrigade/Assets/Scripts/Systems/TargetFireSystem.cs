using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TargetFireSystem : SystemBase
{
    public float timeLeftUntilReload = 5f;

    protected override void OnCreate()
    {
        // Wait for the specified instanciations
        RequireSingletonForUpdate<Heat>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        timeLeftUntilReload -= deltaTime;

        if (timeLeftUntilReload <= 0.0f)
        {
            timeLeftUntilReload = 1.0f;
            var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());
            var gridSize = GetComponent<Spawner>(GetSingletonEntity<Spawner>()).GridSize;

            Entities
                .WithNativeDisableContainerSafetyRestriction(heatSingleton)
                .WithAny<OmniWorker>()
                .ForEach((ref TargetPosition targetPosition, in HeldBucket heldBucket, in Translation currentWorkerPosition) =>
                {
                    // a bucket is held, and water is in the bucket. the target in this case is the fire
                    if (heldBucket.Bucket != Entity.Null && GetComponent<Bucket>(heldBucket.Bucket).HasWater)
                    {
                        targetPosition.Value = currentWorkerPosition.Value; 
                    
                        float closestFirePosition = float.MaxValue;

                        // look at every cell, and if one of them is on fire, look at the distance
                        for (int i = 0; i < heatSingleton.Length; i++)
                        {
                        
                            if (heatSingleton[i].Value > 0.0f)
                            {
                                var xPosition = i % gridSize;
                                var zPosition = i / gridSize;

                                var currentDistance =
                                    math.pow(zPosition - currentWorkerPosition.Value.z, 2) +
                                    math.pow(xPosition - currentWorkerPosition.Value.x, 2);

                                if (closestFirePosition > currentDistance)
                                {
                                    closestFirePosition = currentDistance;
                                    targetPosition.Value = new float3(xPosition, 1, zPosition);
                                }
                            }
                        }
                    }
                }).ScheduleParallel();
        }
    }
}