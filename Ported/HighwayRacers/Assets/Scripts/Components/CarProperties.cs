using Unity.Entities;


struct CarProperties : IComponentData
{
    public float desiredSpeed;
    public float overTakePercent;
    public float minDistanceInFront;
    public float leftMergeDistance;
    public float mergeSpace;
    public float overTakeEagerness;
	public float defaultSpeed;

    public float acceleration;
    public float braking;
}
