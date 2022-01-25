using Unity.Entities;

public partial class CarMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        
        Entities
            .WithAll<SplinePosition,SplineDef, Speed>()
            .ForEach((Entity entity, ref SplinePosition splinePosition, in SplineDef splineDef, in Speed speed) =>
            {
                var splineVector = (splineDef.endPoint - splineDef.startPoint).ToVector3();
                splinePosition.position += speed.speed * time * 1/splineVector.magnitude;
                if (splinePosition.position > 1.0f)
                {
                    splinePosition.position = 0f;
                }
            }).ScheduleParallel();
    }
}
