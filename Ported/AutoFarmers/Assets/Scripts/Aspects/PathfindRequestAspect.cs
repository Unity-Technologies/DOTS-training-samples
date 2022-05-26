using Unity.Entities;

readonly partial struct PathfindingRequestAspectFarmer : IAspect<PathfindingRequestAspectFarmer>
{
    public readonly Entity Self;

    public readonly RefRO<Farmer> Farmer;
    public readonly RefRW<PathfindingIntent> Intent;
}

readonly partial struct PathfindingRequestAspectDrone : IAspect<PathfindingRequestAspectDrone>
{
    public readonly Entity Self;

    public readonly RefRO<Drone> Drone;
    public readonly RefRW<PathfindingIntent> Intent;
}
