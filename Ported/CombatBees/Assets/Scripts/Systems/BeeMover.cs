using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeMover : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        Entities
            .ForEach((ref Translation translation, in Velocity velocity) => 
        {
            translation.Value = translation.Value + velocity.Value * time;
        }).Schedule();
    }
}
