using Unity.Entities;

namespace MetroECS.Trains
{
    [GenerateAuthoringComponent]
    public struct TrainDoor : IComponentData
    {
        public const float DOOR_ACCELERATION = 0.0015f;
        public const float DOOR_FRICTION = 0.9f;
        public const float DOOR_ARRIVAL_THRESHOLD = 0.001f;
        
        public Entity DoorLeft;
        public Entity DoorRight;
    }
}