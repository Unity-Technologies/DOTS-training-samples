using Unity.Entities;

public enum TrainState
{
	Moving,
	Stopped
}

public struct Train : IComponentData
{
	public TrainState TrainState;
}
