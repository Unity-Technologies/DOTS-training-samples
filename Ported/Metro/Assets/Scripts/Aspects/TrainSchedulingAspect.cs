using Unity.Entities;

public readonly partial struct TrainSchedulingAspect : IAspect
{
    public readonly RefRW<Train> train;
    public readonly RefRW<TrainScheduleInfo> schedule;
    public readonly DestinationAspect targetDestination;
}
