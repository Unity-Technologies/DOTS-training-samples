using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveCarOnLaneSystem))]
public class CarPositionToWorldSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, ref Spline spline, ref Lane lane, ref DynamicBuffer<CarBufferElement> buffer) =>
            {
                foreach (Entity car in buffer.Reinterpret<Entity>())
                {
                    CarPosition carPosition = GetComponent<CarPosition>(car);
                    float splineRatio = carPosition.Value / lane.Length;
                    float3 pos = math.lerp(spline.startPos, spline.endPos, splineRatio);
                    Translation translation = new Translation {Value = pos}; 
                    SetComponent(car, translation);
                }
                
            }).Run();
    }
}
