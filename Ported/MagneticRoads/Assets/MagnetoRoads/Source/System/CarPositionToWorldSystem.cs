using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveCarOnLaneSystem))]
public class CarPositionToWorldSystem : SystemBase
{
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<Translation> translationAccessor = GetComponentDataFromEntity<Translation>();
        
        Entities
            .WithNativeDisableContainerSafetyRestriction(translationAccessor)
            .ForEach((Entity entity, ref Spline spline, ref Lane lane, ref DynamicBuffer<CarBufferElement> buffer) =>
            {
                foreach (Entity car in buffer.Reinterpret<Entity>())
                {
                    CarPosition carPosition = GetComponent<CarPosition>(car);
                    float splineRatio = carPosition.Value / lane.Length;

                    float3 pos = BezierUtility.EvaluateBezier(spline.startPos, spline.anchor1, spline.anchor2,
                        spline.endPos, splineRatio);
                    Translation translation = new Translation {Value = pos};
                    translationAccessor[car] = translation;
                }
                
            }).ScheduleParallel();
    }
}
