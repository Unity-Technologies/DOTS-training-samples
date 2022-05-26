using Unity.Entities;


struct CarPosition : IComponentData
{
    public float distance;  // How far along the lane are we
    public int currentLane;  //Change to enum when we have the lanes available?
}