using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CarriageMovementSystem : SystemBase
{
    private int offset = 1;
    
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<TrackPositionComponent> trackPositions =
            GetComponentDataFromEntity<TrackPositionComponent>();
        
        Entities.ForEach((ref CarriageComponent carriage, ref Translation translation) =>
        {
            float trainPosition = trackPositions[carriage.Train].Value;
            
            float3 start = new float3(0, 0, 0); // Replace with curve sampling
            float3 destination = new float3(50, 0, 0);
            
            translation.Value = math.lerp(start, destination, trainPosition);
        }).Schedule();
    }
}