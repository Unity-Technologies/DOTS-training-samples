using Unity.Burst;
using Unity.Entities;

public partial struct LanePopulatingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HighwayConfig>();
        state.RequireForUpdate<CarId>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<HighwayConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        Entity[] carPerLane = new Entity[config.LaneCount];

        foreach(var lane in SystemAPI.Query<LaneAspect>())
        {
            carPerLane[lane.Lane - 1] = lane.self;
        }

        foreach(var car in SystemAPI.Query<CarAspect>())
        {
            ecb.AppendToBuffer(carPerLane[car.Lane - 1], new CarElement { CarEntity = car.carEntity });
        }

        state.Enabled = false;
    }
}
