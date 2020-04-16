using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct Bucket : IComponentData
{
    public float Fill;
}

public struct BucketWorkerRef : IComponentData
{
    public Entity WorkerRef;
}

[UpdateAfter(typeof(WorkerMoveToSystem))]
public class BucketCarrySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var getWorkerPosition = GetComponentDataFromEntity<Translation>(true);
        Entities.WithNativeDisableContainerSafetyRestriction(getWorkerPosition).ForEach((ref Translation tx, in BucketWorkerRef workerRef) => {
            tx = getWorkerPosition[workerRef.WorkerRef];
            tx.Value.y = 4.0f;
        }).ScheduleParallel();
    }
}
