using Unity.Entities;

struct CarriageDoor : IComponentData
{
    public Entity door_LEFT, door_RIGHT;
    public float left_OPEN_X, left_CLOSED_X;
    public float door_SPEED; // Initialized to 0.

    //public CommuterNavPoint door_navPoint;

}

