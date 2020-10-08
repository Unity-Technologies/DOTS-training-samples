using Unity.Entities;
using Unity.Mathematics;

public struct CommuterTask_WaitInCarriage : IComponentData
{
    public Entity Carriage;
}