using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

using src.Components;

public class OmniWorkerSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        var updateWorkersWithoutBucket = Entities
            .WithAll<OmniWorkerTag>()
            .WithNone<WorkerIsHoldingBucket>()            
            .ForEach((ref Translation translation, ref Position position) =>
            {
                // Get nearest empty bucket
                // If close enough to the bucket
                //     pick it up
                // else
                //     move towards the nearest bucket
            }).ScheduleParallel(Dependency);

        var updateWorkerWithBucket = Entities
            .WithAll<OmniWorkerTag>()
            .WithAll<WorkerIsHoldingBucket>()
            .ForEach((ref Translation translation, ref Position position) =>
            {
                // Get bucket reference
                // If bucket is full
                //     Get nearest fire spot
                //     If not close enough to the fire spot 
                //         move towards the fire spot                   
                // Else
                // Get nearest water spot
                // If not close enough to the spot 
                //      move towards the water spot                
            }).ScheduleParallel(updateWorkersWithoutBucket);

        Dependency = updateWorkerWithBucket;
    }
}
