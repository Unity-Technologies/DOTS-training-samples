using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(SimpleIntersectionSystem))]
public class SimpleIntersectionActiveCarSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var positionAccessor = GetComponentDataFromEntity<CarPosition>();
        var translationAccessor = GetComponentDataFromEntity<Translation>();
        
        Entities
            .WithNone<IntersectionNeedsInit>()
            .WithNativeDisableContainerSafetyRestriction(positionAccessor)
            .WithNativeDisableContainerSafetyRestriction(translationAccessor)
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection, in Spline spline) =>
            {
                if (simpleIntersection.car == Entity.Null)
                    return;
                
                var carPosition = positionAccessor[simpleIntersection.car];
                if (carPosition.Value < 1)
                {
                    var carSpeed = GetComponent<CarSpeed>(simpleIntersection.car);
                    float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                    positionAccessor[simpleIntersection.car] = new CarPosition {Value = newPosition};
                    var eval = BezierUtility.EvaluateBezier(spline.startPos, spline.anchor1, spline.anchor2, spline.endPos, newPosition);
                    translationAccessor[simpleIntersection.car] = new Translation {Value = eval};
                }
            }).ScheduleParallel();
    }
}
