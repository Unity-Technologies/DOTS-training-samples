using Unity.Entities;
using Unity.Mathematics;

public struct AIState : IComponentData
{
    public enum State
    {
        Thinking = 0,
        MovingToTarget,
    }

    public State state;
    
    public float SecondsSinceClicked;

    public float3 TargetPosition;
}
