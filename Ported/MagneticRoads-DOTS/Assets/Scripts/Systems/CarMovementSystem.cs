using Unity.Entities;
using Unity.Collections;

[UpdateInGroup(typeof(CarMovementGroup))]
public partial class CarMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Persistent);

        Entities
            .WithAll<SplinePosition, SplineDef, Speed>()
            .WithNone<RoadCompleted>()
            .ForEach((Entity entity, ref SplinePosition splinePosition, in SplineDef splineDef, in Speed speed) =>
            {
                var splineVector = (splineDef.endPoint - splineDef.startPoint).ToVector3();
                splinePosition.position += speed.speed * time * 1 / splineVector.magnitude;
                if (splinePosition.position > 1.0f)
                {
                    splinePosition.position = 1.0f;
                    ecb.AddComponent<RoadCompleted>(entity);
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
