using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ConstraintsSystem))]
public partial class RenderPrepSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var worldEntity = GetSingletonEntity<World>();
        var currentPointBuffer = GetBuffer<CurrentPoint>(worldEntity);


        Entities
            .WithReadOnly(currentPointBuffer)
            .ForEach((Entity entity, ref Beam beam, ref LocalToWorld localToWorld) =>
        { 
            //TODO: cache positions in beams?
            var pointA = currentPointBuffer[beam.pointAIndex];
            var pointB = currentPointBuffer[beam.pointBIndex];
             
            //TODO: separate from random access generation 
            var direction = math.normalize(pointB.Value - pointA.Value);
            localToWorld.Value = float4x4.TRS(pointB.Value + (pointA.Value - pointB.Value) / 2f, quaternion.LookRotation(direction, new float3(0f, 1f, 0f)), new float3(.25f, .25f, beam.size));
             

        }).ScheduleParallel(
            
        );

        //TODO: complete in the optimal place
        Dependency.Complete();


    }
}
