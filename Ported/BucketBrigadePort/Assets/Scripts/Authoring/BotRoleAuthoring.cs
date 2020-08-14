using Unity.Entities;
using Unity.Mathematics;

public struct BotRoleFinder : IComponentData
{
    public Entity Dependent;
}
public struct BotRoleTosser : IComponentData
{
    public Entity Dependent;
    public float3 RootPosition;
}
public struct BotRoleFiller : IComponentData
{
    public Entity Dependent;
    public float3 RootPosition;
}
public struct BotRolePasserFull : IComponentData
{
    public Entity Dependent;
    public int Id;
    public float3 RootPosition;
}
public struct BotRolePasserEmpty : IComponentData
{
    public Entity Dependent;
    public int Id;
    public float3 RootPosition;

}
public struct BotRoleOmni : IComponentData
{
}