using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class AntMovementSystem : SystemBase
{
    //private EntityQuery query;
    protected override void OnUpdate()
    {
        //int dataCount = query.CalculateEntityCount();
        //NativeArray<float> dataSquared
        //    = new NativeArray<float>(dataCount, Allocator.Temp);
        //Entities
        //    .WithStoreEntityQueryInField(ref query)
        //    .ForEach((int entityInQueryIndex, in AntMovement data) =>
        //    {
        //        Debug.Log(data.Position);
        //    }).ScheduleParallel();

        //Job
        //    .WithCode(() =>
        //    {
        //    //Use dataSquared array...
        //    var v = dataSquared[dataSquared.Length - 1];
        //    })
        //    .WithDisposeOnCompletion(dataSquared)
        //    .Schedule();
    }
}
