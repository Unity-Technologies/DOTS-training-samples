using Unity.Entities;
using UnityEngine;

public enum TrainState
{
    Idle,
    DoorOpening,
    DoorClosing,
    Boarding,
    TrainMovement
}

public class TrainAuthoring : MonoBehaviour
{
    public int CarriageCount;

    class Baker : Baker<TrainAuthoring>
    {
        public override void Bake(TrainAuthoring authoring)
        {
            AddComponent(new Train()
            {
                CarriageCount = authoring.CarriageCount
            });
        }
    }
}

struct Train : IComponentData
{
    public Entity Line;
    public TrainState State;
    public int CarriageCount;
}