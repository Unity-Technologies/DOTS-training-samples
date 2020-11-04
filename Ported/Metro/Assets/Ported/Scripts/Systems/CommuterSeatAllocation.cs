using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterSeatAllocation : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<WaitingForTrainTag>()
            .ForEach((in Commuter commuter, in PlatformID platformID) =>
            {
                // var carriage = GetEmptiestCarriage(stationID)
                // var seats = GetAvailableSeats(carriageID)
                // var seat = AllocateSeat(seats[random], commuter)
            }).ScheduleParallel();
    }
}

public class CommuterMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer();
        var deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Translation translation, in Commuter commuter, in DynamicBuffer<MoveTarget> targets) =>
        {
            var currentTarget = targets[0];
            if (math.distance(translation.Value, currentTarget.Position) < 0.1f)
            {
                // ecb.RemoveComponent(currentTarget);
            }
            else
            {
                translation.Value = math.lerp(translation.Value, currentTarget.Position, deltaTime);
            }
        }).Schedule();
    }
}

public struct Commuter : IComponentData
{
}

public struct WaitingForTrainTag : IComponentData
{
    
}

public struct PlatformID : IComponentData
{
    public int Value;
}

public struct TrackID : IComponentData
{
    public int Value;
}

public struct MoveTarget : IBufferElementData
{
    public float3 Position;
}