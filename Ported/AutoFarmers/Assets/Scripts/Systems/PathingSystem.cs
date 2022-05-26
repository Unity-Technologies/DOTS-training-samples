
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PathingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        
        var updatePathJob = new UpdatePathJob()
        {
            InRangeDistance = 0.1f,
            MapWidth = gameConfig.MapSize.x,
            MapHeight = gameConfig.MapSize.y,
        };
        
        updatePathJob.Schedule();
    }
}

// Pops out the last waypoint in a gridmover's waypoints if they are within a certain range of it
[BurstCompile]
partial struct UpdatePathJob : IJobEntity
{
    public float InRangeDistance;
    public int MapWidth;
    public int MapHeight;
    
    void Execute(ref PathingAspect pathingAgent)
    {
        if (pathingAgent.Waypoints.Length > 0)
        {
            float3 agentTranslation3 = pathingAgent.Movement.Transform.Position;
            float2 agentTranslation = new float2(agentTranslation3.x, agentTranslation3.y);
        
            Waypoint nextWaypoint = pathingAgent.Waypoints[pathingAgent.Waypoints.Length - 1];
            float2 nextWaypointTranslation = GroundUtilities.GetTileTranslation(nextWaypoint.TileIndex, MapWidth);
        
            if (math.distance(agentTranslation, nextWaypointTranslation) < InRangeDistance)
            {
                pathingAgent.Waypoints.RemoveAt(pathingAgent.Waypoints.Length - 1);
            }

            if (pathingAgent.Waypoints.Length > 0)
            {
                pathingAgent.Movement.DesiredLocation = GroundUtilities.GetTileCoords(pathingAgent.Waypoints[pathingAgent.Waypoints.Length - 1].TileIndex, MapWidth);
                pathingAgent.Movement.HasDestination = true;
            }
            else
            {
                pathingAgent.Movement.HasDestination = false;
            }

        }
    }
}
