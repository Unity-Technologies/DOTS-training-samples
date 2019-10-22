using Unity.Entities;

struct CarState : IComponentData
{
    public float FwdSpeed; // velocityPosition
    public float LeftSpeed; // velocityLane

    public int SpacePartitionIndex;

    public enum State {
        NORMAL,
        MERGE_RIGHT,
        MERGE_LEFT,
        OVERTAKING,
    }
    public State CurrentState;
}
