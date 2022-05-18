using Unity.Entities;

public enum TrainState
{
	Moving,
	Stopped,
	Leaving,
}

public struct Train : IComponentData
{
	public TrainState TrainState;
	public float WaitTimer;
}
