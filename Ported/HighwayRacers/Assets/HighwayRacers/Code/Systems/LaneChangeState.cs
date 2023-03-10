using Unity.Entities;

// Enableable component type
public struct LaneChangeState : IComponentData
{
    public bool requestChangeUp;
    public bool approveChangeUp;
    public bool requestChangeDown;
    public bool approveChangeDown;
    public float distFromCarInFront;
    public int myIndex;
}