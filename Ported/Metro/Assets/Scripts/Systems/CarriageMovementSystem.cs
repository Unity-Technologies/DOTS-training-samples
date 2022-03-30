using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CarriageMovementSystem : SystemBase
{
    private int offset = 1;
    
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CarriageComponent carriage, ref Translation translation) =>
        {
            float trainPosition = GetComponent<TrackPositionComponent>(carriage.Train).Value;
            trainPosition += carriage.Index * 0.01f;
            
            float3 start = new float3(0, 0, 0); // Replace with curve sampling
            float3 destination = new float3(50, 0, 0);
            
            translation.Value = math.lerp(start, destination, trainPosition);
        }).Schedule();
    }
}