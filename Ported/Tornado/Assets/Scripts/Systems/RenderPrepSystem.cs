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


        Entities.ForEach((
            Entity entity, 
            ref Beam beam, 
            ref Translation translation, 
            ref NonUniformScale scale, 
            ref Rotation rotation) =>
        {
            var pointA = currentPointBuffer[beam.pointAIndex];
            var pointB = currentPointBuffer[beam.pointBIndex];

            translation.Value = pointB.Value + (pointA.Value - pointB.Value) / 2f;
            scale.Value = new float3(.25f, .25f, beam.size);

            var direction = math.normalize(pointB.Value - pointA.Value);
            rotation.Value = quaternion.LookRotation(direction, new float3(0f, 1f, 0f));

        }).Schedule();
    }
}
