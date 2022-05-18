using Unity.Entities;

public enum TrainState
{
	Moving,
	Stopping,
	Stopped,
}

public struct Train : IComponentData
{
	public TrainState TrainState;
	public float DepartureTime;
	public float SpeedPercentage;
}
