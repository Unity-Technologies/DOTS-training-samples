using Unity.Entities;
using UnityEngine;

public enum CommuterState
{
    Idle,
    InTrain,
    Boarding,
    Unboarding,
    MoveToDestination,
    Queueing
}

public class CommuterAuthoring : MonoBehaviour
{
    class Baker : Baker<CommuterAuthoring>
    {
        public override void Bake(CommuterAuthoring authoring)
        {
            AddComponent<Commuter>();
        }
    }
}

struct Commuter : IComponentData
{
    public CommuterState State;
}
